Project: AkifNOW

- Build a single Windows desktop app in Python named AkifNOW.
- GUI shows two buttons on launch: Client and Server.
- Default port: 9986.
- Show server IP on Server mode screen.
- Client mode shows IP entry.
- Stream video + audio with minimum latency, high quality like GeForce NOW.
- Target default: 1920x1080 @ 60 fps; allow client-selectable resolution and fps; server adapts.
- Capture mouse + keyboard on client and send to server; server injects input.
- Use as much bandwidth as needed; prioritize quality and low latency.
- No extra scaffolding files; keep code self-contained; no unnecessary comments in code.
- Use aiortc for WebRTC transport (audio/video/datachannel), mss for screen capture, sounddevice for audio capture/playback, pynput for input, PyQt6 for UI.
- Single executable entry point that embeds both Client and Server.
- Port is fixed to 9986 unless user changes settings.
- Provide settings UI to pick resolution (e.g., 1280x720, 1920x1080) and fps (30, 60) before connecting.
