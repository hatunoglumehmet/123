# Build Instructions

## Prerequisites
1. **Visual Studio 2019/2022** with .NET Framework 4.7.2 support
2. **Mount & Blade II: Bannerlord** installed
3. **Harmony** and **MCM v5** mods installed

## Build Steps

### Method 1: Visual Studio
1. Open `src/EnhancedDiplomacyWarfare/EnhancedDiplomacyWarfare.csproj`
2. Update Bannerlord reference paths in the project file if needed
3. Build → Build Solution (Ctrl+Shift+B)
4. Output will be in `bin/Win64_Shipping_Client/`

### Method 2: Command Line
```bash
# Navigate to project directory
cd src/EnhancedDiplomacyWarfare/

# Restore NuGet packages
dotnet restore

# Build the project
dotnet build --configuration Release

# Or build directly with MSBuild
msbuild EnhancedDiplomacyWarfare.csproj /p:Configuration=Release
```

## Configuration
Update the Bannerlord installation path in the .csproj file:
```xml
<Reference Include="TaleWorlds.Core">
  <HintPath>YOUR_BANNERLORD_PATH\bin\Win64_Shipping_Client\TaleWorlds.Core.dll</HintPath>
  <Private>False</Private>
</Reference>
```

Common paths:
- Steam: `C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\`
- Epic: `C:\Program Files\Epic Games\Mount & Blade II Bannerlord\`

## Testing
1. Copy built files to Bannerlord Modules folder
2. Enable mod in launcher
3. Start new campaign or load existing save
4. Test features through MCM menu

## Development
- Use Visual Studio debugger by attaching to Bannerlord process
- Enable debug mode in MCM for detailed logging
- Use Harmony debugging tools for patch troubleshooting