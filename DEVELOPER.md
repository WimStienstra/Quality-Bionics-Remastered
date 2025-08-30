# Developer Reference - Quality Bionics Continued

## Project Structure Overview

This project has been completely restructured to follow Visual Studio conventions with a single, unified project using the `QualityBionicsRemastered` namespace throughout.

### Key Files

- **`QualityBionics-Continued.sln`** - Main Visual Studio solution file
- **`Source/QualityBionicsRemastered/QualityBionicsRemastered.csproj`** - Single unified project
- **`build.bat`** - Cross-platform build script
- **`.vscode/build.ps1`** - VS Code/PowerShell build script

### Unified Namespace Structure

- **Primary Namespace**: `QualityBionicsRemastered`
  - Used consistently throughout the entire codebase
  - Maintains compatibility across future forks

- **Backward Compatibility**: Implemented through wrapper classes in both the `QualityBionics` namespace and the `QualityBionicsContinued` namespace
  - Located in `BackwardCompatibility/EBF_BackCompatibility.cs`
  - Provides compatibility for EBF 1.5 expecting the old namespace
  - Both `QualityBionicsContinued.dll` and `QualityBionicsRemastered.dll` are generated (identical content)

### Building the Project

#### Option 1: Visual Studio
1. Open `QualityBionics-Continued.sln`
2. Set configuration to "Release" 
3. Build → Build Solution

#### Option 2: Command Line
```cmd
# Build for both RimWorld versions
.\build.bat

# Build specific version
set RimWorldVersion=1.5
dotnet build QualityBionics-Continued.sln --configuration Release
```

#### Option 3: VS Code
- Use Ctrl+Shift+P → "Tasks: Run Task" → "build dll"
- Or use the build task in the integrated terminal

### Output Structure

Built assemblies are placed in:
- `1.5/Assemblies/` - RimWorld 1.5 compatible assemblies
- `1.6/Assemblies/` - RimWorld 1.6 compatible assemblies

Both folders contain:
- `QualityBionicsRemastered.dll` - Main assembly
- `QualityBionicsContinued.dll` - Backward compatibility copy (identical to main assembly)

### Dependencies

The project uses:
- **.NET Framework 4.7.2** - Required by RimWorld
- **Krafs.Rimworld.Ref** - RimWorld API references (version-specific)
- **Lib.Harmony 2.2.2** - Runtime patching framework
- **Vectorial1024.EliteBionicsFrameworkAPI** - Minimal Elite Bionics Framework references (standalone)

### Multi-Version Support

The project supports building for multiple RimWorld versions through:
- Conditional compilation symbols (`v1_5`, `v1_6`)
- Environment variable `RimWorldVersion`
- Version-specific package references

### Development Notes

1. **Unified Namespace**: All new code uses `QualityBionicsRemastered` namespace consistently
2. **Single Project**: No more multiple project complexity - everything builds from one project
3. **Legacy Support**: Automatically maintained through wrapper classes, for RW 1.5 users
4. **Visual Studio Integration**: Full IntelliSense and debugging support

### Troubleshooting

- **Build Errors**: Ensure .NET Framework 4.7.2 SDK is installed
- **Missing References**: Run `dotnet restore` to restore NuGet packages
- **Version Issues**: Check that `RimWorldVersion` environment variable is set correctly
- **Path Issues**: Ensure you're building from the project root directory

### Mod Compatibility Information
Compatibility with Elite Bionics Framework for RW 1.5 is still managed by EBF, as per usual for previous Quality Bionics editions.
