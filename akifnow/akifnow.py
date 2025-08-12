import sys
import os
import json
import asyncio
import socket
import struct
import time
import threading
import queue
import numpy as np
from PyQt6.QtWidgets import QApplication,QMainWindow,QWidget,QPushButton,QVBoxLayout,QHBoxLayout,QLabel,QLineEdit,QComboBox,QStackedWidget,QMessageBox
from PyQt6.QtCore import Qt,pyqtSignal,QObject,QSize
from PyQt6.QtGui import QImage,QPixmap
from aiortc import RTCPeerConnection,RTCSessionDescription,RTCIceCandidate,MediaStreamTrack,RTCRtpSender
from aiortc.mediastreams import AudioStreamTrack,VideoStreamTrack
import av
import mss
import sounddevice as sd
from pynput import mouse,keyboard
import psutil

class AsyncRunner:
    def __init__(self):
        self.loop=asyncio.new_event_loop()
        self.thread=threading.Thread(target=self.loop.run_forever,daemon=True)
        self.thread.start()
    def run(self,coro):
        return asyncio.run_coroutine_threadsafe(coro,self.loop)
    def call(self,func,*args,**kwargs):
        def f():
            func(*args,**kwargs)
        self.loop.call_soon_threadsafe(f)
    def stop(self):
        def stopper():
            for task in asyncio.all_tasks(loop=self.loop):
                task.cancel()
            self.loop.stop()
        self.loop.call_soon_threadsafe(stopper)

async def wait_ice_gathering_complete(pc: RTCPeerConnection, timeout: float = 5.0):
    if pc.iceGatheringState == 'complete':
        return
    done=asyncio.Event()
    @pc.on('icegatheringstatechange')
    def _():
        if pc.iceGatheringState == 'complete':
            done.set()
    try:
        await asyncio.wait_for(done.wait(), timeout)
    except asyncio.TimeoutError:
        pass

class ScreenVideoTrack(VideoStreamTrack):
    def __init__(self,width,height,fps):
        super().__init__()
        self.width=width
        self.height=height
        self.fps=fps
        self._frame_time=1.0/max(1,fps)
        self._last=0
        self._sct=mss.mss()
        self._mon=self._sct.monitors[1]
    async def recv(self):
        now=time.time()
        wait=self._frame_time-(now-self._last)
        if wait>0:
            await asyncio.sleep(wait)
        img=self._sct.grab(self._mon)
        arr=np.asarray(img,dtype=np.uint8)
        frame=av.VideoFrame.from_ndarray(arr,format='bgra')
        frame=frame.reformat(width=self.width,height=self.height,format='yuv420p')
        self._last=time.time()
        return frame

class AudioSourceTrack(AudioStreamTrack):
    def __init__(self,samplerate=48000,channels=2,device=None,loopback=False):
        super().__init__()
        self.samplerate=samplerate
        self.channels=channels
        self.q=asyncio.Queue()
        self.stream=None
        self.frames_per_chunk=960
        def cb(indata,frames,time_info,status):
            try:
                data=np.copy(indata)
                if data.dtype!=np.int16:
                    data=(data*32767.0).astype(np.int16)
                self._enqueue(data)
            except Exception:
                pass
        try:
            if loopback and os.name=='nt':
                hostapis=sd.query_hostapis()
                wasapi_index=None
                for i,h in enumerate(hostapis):
                    if 'wasapi' in h.get('name','').lower():
                        wasapi_index=i
                        break
                dev_index=None
                if wasapi_index is not None:
                    devs=sd.query_devices()
                    for i,d in enumerate(devs):
                        if d.get('hostapi')==wasapi_index and d.get('max_input_channels',0)>0 and 'loopback' in d.get('name','').lower():
                            dev_index=i
                            break
                if dev_index is not None:
                    self.stream=sd.InputStream(device=dev_index,samplerate=self.samplerate,channels=self.channels,dtype='int16',blocksize=self.frames_per_chunk,callback=cb)
                else:
                    self.stream=sd.InputStream(samplerate=self.samplerate,channels=self.channels,dtype='int16',blocksize=self.frames_per_chunk,callback=cb)
            else:
                self.stream=sd.InputStream(samplerate=self.samplerate,channels=self.channels,dtype='int16',blocksize=self.frames_per_chunk,callback=cb)
            self.stream.start()
        except Exception:
            self.stream=None
    def _enqueue(self,data):
        try:
            self.q.put_nowait(data)
        except asyncio.QueueFull:
            pass
    async def recv(self):
        try:
            data=await self.q.get()
        except Exception:
            data=np.zeros((self.frames_per_chunk,self.channels),dtype=np.int16)
        if len(data.shape)==1:
            data=np.expand_dims(data,axis=1)
        if data.shape[1]!=self.channels:
            data=np.tile(data,(1,self.channels))
        af=av.AudioFrame.from_ndarray(data.T,format='s16',layout='stereo')
        af.sample_rate=self.samplerate
        return af
    async def stop(self):
        if self.stream:
            try:
                self.stream.stop()
                self.stream.close()
            except Exception:
                pass

class VideoWidget(QLabel):
    mouseEvent=pyqtSignal(dict)
    keyEvent=pyqtSignal(dict)
    def __init__(self):
        super().__init__()
        self.setAlignment(Qt.AlignmentFlag.AlignCenter)
        self.setMouseTracking(True)
        self.setFocusPolicy(Qt.FocusPolicy.StrongFocus)
    def sizeHint(self):
        return QSize(960,540)
    def setFrame(self,image:QImage):
        self.setPixmap(QPixmap.fromImage(image))
    def _norm(self,pos):
        w=self.width()
        h=self.height()
        x=max(0,min(1,pos.x()/max(1,w-1)))
        y=max(0,min(1,pos.y()/max(1,h-1)))
        return x,y
    def mouseMoveEvent(self,e):
        x,y=self._norm(e.position())
        self.mouseEvent.emit({"type":"move","x":x,"y":y})
    def mousePressEvent(self,e):
        x,y=self._norm(e.position())
        b="left" if e.button()==Qt.MouseButton.LeftButton else "right" if e.button()==Qt.MouseButton.RightButton else "middle"
        self.mouseEvent.emit({"type":"down","button":b,"x":x,"y":y})
    def mouseReleaseEvent(self,e):
        x,y=self._norm(e.position())
        b="left" if e.button()==Qt.MouseButton.LeftButton else "right" if e.button()==Qt.MouseButton.RightButton else "middle"
        self.mouseEvent.emit({"type":"up","button":b,"x":x,"y":y})
    def wheelEvent(self,e):
        delta=e.angleDelta().y()
        self.mouseEvent.emit({"type":"wheel","delta":int(delta)})
    def keyPressEvent(self,e):
        k=e.key()
        t=e.text()
        self.keyEvent.emit({"type":"keydown","key":int(k),"text":t})
    def keyReleaseEvent(self,e):
        k=e.key()
        t=e.text()
        self.keyEvent.emit({"type":"keyup","key":int(k),"text":t})

class SignalingServer:
    def __init__(self,runner,host='0.0.0.0',port=9986,on_client=None):
        self.runner=runner
        self.host=host
        self.port=port
        self.on_client=on_client
        self.server=None
    async def start(self):
        async def handle(reader,writer):
            if self.on_client:
                await self.on_client(reader,writer)
        self.server=await asyncio.start_server(handle,self.host,self.port)
    async def stop(self):
        if self.server:
            self.server.close()
            await self.server.wait_closed()
            self.server=None

class ServerController(QObject):
    status=pyqtSignal(str)
    def __init__(self,runner):
        super().__init__()
        self.runner=runner
        self.signaling=None
        self.pc=None
        self.video=None
        self.audio=None
        self.dc=None
        self.screen_size=self._get_screen_size()
    def _get_screen_size(self):
        s=mss.mss()
        mon=s.monitors[1]
        return mon["width"],mon["height"]
    def start(self):
        async def on_client(reader,writer):
            try:
                line=await reader.readline()
                cfg=json.loads(line.decode().strip())
                if cfg.get('type')!='hello':
                    writer.close()
                    await writer.wait_closed()
                    return
                w,h=cfg.get('width',1920),cfg.get('height',1080)
                fps=cfg.get('fps',60)
                self.pc=RTCPeerConnection()
                self.video=ScreenVideoTrack(w,h,fps)
                self.audio=AudioSourceTrack(loopback=True)
                self.pc.addTrack(self.video)
                self.pc.addTrack(self.audio)
                @self.pc.on('datachannel')
                def on_dc(channel):
                    self.dc=channel
                    @channel.on('message')
                    def on_msg(message):
                        try:
                            evt=json.loads(message)
                            self._handle_input(evt)
                        except Exception:
                            pass
                offer=await self.pc.createOffer()
                await self.pc.setLocalDescription(offer)
                await wait_ice_gathering_complete(self.pc)
                msg=json.dumps({"type":"offer","sdp":self.pc.localDescription.sdp,"sdpType":self.pc.localDescription.type})+"\n"
                writer.write(msg.encode())
                await writer.drain()
                while True:
                    line=await reader.readline()
                    if not line:
                        break
                    data=json.loads(line.decode().strip())
                    if data.get('type')=='answer':
                        rd=RTCSessionDescription(sdp=data['sdp'],type=data['sdpType'])
                        await self.pc.setRemoteDescription(rd)
                try:
                    await self.pc.close()
                except Exception:
                    pass
            except Exception:
                pass
            try:
                writer.close()
                await writer.wait_closed()
            except Exception:
                pass
        async def start_async():
            self.signaling=SignalingServer(self.runner,on_client=on_client)
            await self.signaling.start()
            self.status.emit('Listening on 0.0.0.0:9986')
        self.runner.run(start_async())
    def stop(self):
        async def stop_async():
            try:
                if self.signaling:
                    await self.signaling.stop()
                if self.pc:
                    await self.pc.close()
                if self.audio:
                    await self.audio.stop()
            except Exception:
                pass
        self.runner.run(stop_async())
    def _handle_input(self,evt):
        try:
            et=evt.get('type')
            if et in ['move','down','up']:
                x=evt.get('x',0.5)
                y=evt.get('y',0.5)
                sx=int(x*(self.screen_size[0]-1))
                sy=int(y*(self.screen_size[1]-1))
                mc=mouse.Controller()
                if et=='move':
                    mc.position=(sx,sy)
                else:
                    b=evt.get('button','left')
                    bb=mouse.Button.left if b=='left' else mouse.Button.right if b=='right' else mouse.Button.middle
                    mc.position=(sx,sy)
                    if et=='down':
                        mc.press(bb)
                    else:
                        mc.release(bb)
            elif et=='wheel':
                delta=evt.get('delta',0)
                mc=mouse.Controller()
                mc.scroll(0,int(delta/120))
            elif et in ['keydown','keyup']:
                kc=evt.get('key')
                txt=evt.get('text','')
                kb=keyboard.Controller()
                if txt and len(txt)==1:
                    if et=='keydown':
                        kb.press(txt)
                    else:
                        kb.release(txt)
        except Exception:
            pass

class ClientController(QObject):
    status=pyqtSignal(str)
    frameReady=pyqtSignal(QImage)
    def __init__(self,runner,video_widget):
        super().__init__()
        self.runner=runner
        self.pc=None
        self.dc=None
        self.reader=None
        self.writer=None
        self.audio_task=None
        self.video_task=None
        self.video_widget=video_widget
        self.play_stream=None
        self.play_queue=asyncio.Queue()
        self.connected=False
    def connect(self,ip,width,height,fps):
        async def run():
            try:
                self.pc=RTCPeerConnection()
                self.dc=self.pc.createDataChannel('input')
                @self.pc.on('track')
                def on_track(track):
                    if track.kind=='video':
                        asyncio.ensure_future(self._consume_video(track))
                    elif track.kind=='audio':
                        asyncio.ensure_future(self._consume_audio(track))
                self.reader,self.writer=await asyncio.open_connection(ip,9986)
                hello=json.dumps({"type":"hello","width":width,"height":height,"fps":fps})+"\n"
                self.writer.write(hello.encode())
                await self.writer.drain()
                while True:
                    line=await self.reader.readline()
                    if not line:
                        break
                    data=json.loads(line.decode().strip())
                    if data.get('type')=='offer':
                        rd=RTCSessionDescription(sdp=data['sdp'],type=data['sdpType'])
                        await self.pc.setRemoteDescription(rd)
                        answer=await self.pc.createAnswer()
                        await self.pc.setLocalDescription(answer)
                        await wait_ice_gathering_complete(self.pc)
                        msg=json.dumps({"type":"answer","sdp":self.pc.localDescription.sdp,"sdpType":self.pc.localDescription.type})+"\n"
                        self.writer.write(msg.encode())
                        await self.writer.drain()
                        self.connected=True
                        self.status.emit('Connected')
            except Exception as e:
                self.status.emit('Connection failed')
            finally:
                self.connected=False
        self.runner.run(run())
    def disconnect(self):
        async def run():
            try:
                if self.writer:
                    try:
                        self.writer.close()
                        await self.writer.wait_closed()
                    except Exception:
                        pass
                if self.pc:
                    await self.pc.close()
                self.status.emit('Disconnected')
            except Exception:
                pass
        self.runner.run(run())
    async def _consume_video(self,track:MediaStreamTrack):
        try:
            while True:
                frame=await track.recv()
                img=frame.to_ndarray(format='rgb24')
                h,w,_=img.shape
                qimg=QImage(img.data,w,h,3*w,QImage.Format.Format_RGB888)
                self.frameReady.emit(qimg.copy())
        except Exception:
            pass
    async def _consume_audio(self,track:MediaStreamTrack):
        try:
            out=sd.OutputStream(samplerate=48000,channels=2,dtype='int16')
            out.start()
            while True:
                af=await track.recv()
                p=af.to_ndarray(format='s16').T
                if p.dtype!=np.int16:
                    p=p.astype(np.int16)
                p=p.T
                try:
                    out.write(p)
                except Exception:
                    pass
        except Exception:
            pass
    def send_input(self,evt):
        def f():
            try:
                if self.dc and self.connected:
                    self.dc.send(json.dumps(evt))
            except Exception:
                pass
        self.runner.call(f)

class ServerView(QWidget):
    def __init__(self,runner):
        super().__init__()
        self.runner=runner
        self.ctrl=ServerController(runner)
        self.label_ip=QLabel()
        self.label_status=QLabel()
        self.btn_start=QPushButton('Start Server')
        self.btn_stop=QPushButton('Stop Server')
        self.btn_stop.setEnabled(False)
        v=QVBoxLayout()
        v.addWidget(QLabel('Server IPs'))
        v.addWidget(self.label_ip)
        v.addWidget(self.label_status)
        v.addWidget(self.btn_start)
        v.addWidget(self.btn_stop)
        v.addStretch(1)
        self.setLayout(v)
        self.btn_start.clicked.connect(self.on_start)
        self.btn_stop.clicked.connect(self.on_stop)
        self.ctrl.status.connect(self.on_status)
        self.update_ips()
    def update_ips(self):
        ips=[]
        for name,addrs in psutil.net_if_addrs().items():
            for a in addrs:
                if a.family==socket.AF_INET and not a.address.startswith('127.'):
                    ips.append(a.address)
        if not ips:
            try:
                ips.append(socket.gethostbyname(socket.gethostname()))
            except Exception:
                pass
        self.label_ip.setText('\n'.join(ips)+':9986')
    def on_status(self,msg):
        self.label_status.setText(msg)
    def on_start(self):
        self.ctrl.start()
        self.btn_start.setEnabled(False)
        self.btn_stop.setEnabled(True)
    def on_stop(self):
        self.ctrl.stop()
        self.btn_start.setEnabled(True)
        self.btn_stop.setEnabled(False)

class ClientView(QWidget):
    def __init__(self,runner):
        super().__init__()
        self.runner=runner
        self.video=VideoWidget()
        self.ctrl=ClientController(runner,self.video)
        self.ctrl.frameReady.connect(self.video.setFrame)
        self.ctrl.status.connect(self.on_status)
        self.ip_edit=QLineEdit()
        self.ip_edit.setPlaceholderText('Server IP')
        self.res_combo=QComboBox()
        self.res_combo.addItems(['1280x720','1920x1080'])
        self.res_combo.setCurrentText('1920x1080')
        self.fps_combo=QComboBox()
        self.fps_combo.addItems(['30','60'])
        self.fps_combo.setCurrentText('60')
        self.btn_connect=QPushButton('Connect')
        self.btn_disconnect=QPushButton('Disconnect')
        self.btn_disconnect.setEnabled(False)
        top=QHBoxLayout()
        top.addWidget(QLabel('IP'))
        top.addWidget(self.ip_edit)
        top.addWidget(QLabel('Resolution'))
        top.addWidget(self.res_combo)
        top.addWidget(QLabel('FPS'))
        top.addWidget(self.fps_combo)
        top.addWidget(self.btn_connect)
        top.addWidget(self.btn_disconnect)
        v=QVBoxLayout()
        v.addLayout(top)
        v.addWidget(self.video)
        self.setLayout(v)
        self.btn_connect.clicked.connect(self.on_connect)
        self.btn_disconnect.clicked.connect(self.on_disconnect)
        self.video.mouseEvent.connect(self.on_mouse)
        self.video.keyEvent.connect(self.on_key)
    def on_status(self,msg):
        pass
    def on_connect(self):
        ip=self.ip_edit.text().strip()
        if not ip:
            QMessageBox.warning(self,'AkifNOW','Enter server IP')
            return
        res=self.res_combo.currentText()
        w,h=map(int,res.split('x'))
        fps=int(self.fps_combo.currentText())
        self.ctrl.connect(ip,w,h,fps)
        self.btn_connect.setEnabled(False)
        self.btn_disconnect.setEnabled(True)
    def on_disconnect(self):
        self.ctrl.disconnect()
        self.btn_connect.setEnabled(True)
        self.btn_disconnect.setEnabled(False)
    def on_mouse(self,evt):
        self.ctrl.send_input(evt)
    def on_key(self,evt):
        self.ctrl.send_input(evt)

class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle('AkifNOW')
        self.runner=AsyncRunner()
        self.stack=QStackedWidget()
        self.home=QWidget()
        hb=QHBoxLayout()
        btn_server=QPushButton('Server')
        btn_client=QPushButton('Client')
        hb.addWidget(btn_server)
        hb.addWidget(btn_client)
        self.home.setLayout(hb)
        self.server_view=ServerView(self.runner)
        self.client_view=ClientView(self.runner)
        self.stack.addWidget(self.home)
        self.stack.addWidget(self.server_view)
        self.stack.addWidget(self.client_view)
        self.setCentralWidget(self.stack)
        btn_server.clicked.connect(lambda: self.stack.setCurrentWidget(self.server_view))
        btn_client.clicked.connect(lambda: self.stack.setCurrentWidget(self.client_view))
    def closeEvent(self,e):
        try:
            self.runner.stop()
        except Exception:
            pass
        e.accept()

def main():
    app=QApplication(sys.argv)
    w=MainWindow()
    w.resize(1200,800)
    w.show()
    sys.exit(app.exec())

if __name__=='__main__':
    main()
