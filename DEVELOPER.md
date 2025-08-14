# Developer Reference - Quality Bionics Continued

## Project Structure Overview

This project has been restructured to follow Visual Studio conventions and modern .NET SDK project standards.

### Key Files

- **`QualityBionics-Continued.sln`** - Main Visual Studio solution file
- **`Source/QualityBionicsRemastered/QualityBionicsRemastered.csproj`** - Main mod project
- **`Source/QualityBionics/QualityBionics.csproj`** - Backward compatibility project
- **`build.bat`** - Cross-platform build script
- **`.vscode/build.ps1`** - VS Code/PowerShell build script

### Namespaces

- **Primary Namespace**: `QualityBionicsRemastered`
  - This should be used for all new development
  - Maintains compatibility across future forks
  
- **Legacy Namespace**: `QualityBionics` 
  - Used only for backward compatibility
  - Contains type forwarding for EBF compatibility

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

### Dependencies

The project uses:
- **.NET Framework 4.7.2** - Required by RimWorld
- **Krafs.Rimworld.Ref** - RimWorld API references (version-specific)
- **Lib.Harmony 2.2.2** - Runtime patching framework

### Multi-Version Support

The project supports building for multiple RimWorld versions through:
- Conditional compilation symbols (`v1_5`, `v1_6`)
- Environment variable `RimWorldVersion`
- Version-specific package references

### Development Notes

1. **Namespace Consistency**: Always use `QualityBionicsRemastered` for new code
2. **Clean Builds**: Use `dotnet clean` before building if you encounter issues
3. **Visual Studio Integration**: The solution is now fully compatible with Visual Studio IntelliSense and debugging
4. **Backward Compatibility**: The `QualityBionics` project maintains compatibility with existing saves and other mods

### Troubleshooting

- **Build Errors**: Ensure .NET Framework 4.7.2 SDK is installed
- **Missing References**: Run `dotnet restore` to restore NuGet packages
- **Version Issues**: Check that `RimWorldVersion` environment variable is set correctly
- **Path Issues**: Ensure you're building from the project root directory
