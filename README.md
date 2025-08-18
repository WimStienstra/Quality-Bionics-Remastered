[![RimWorld 1.6](https://img.shields.io/badge/RimWorld-1.6-brightgreen.svg)](http://rimworldgame.com/) [![Build](https://github.com/WimStienstra/Quality-Bionics-Continued/actions/workflows/build.yml/badge.svg)](https://github.com/WimStienstra/Quality-Bionics-Continued/actions/workflows/build.yml)

# Quality Bionics Remastered

**Originally created by RebelRabbit, continued by Taranchuk and ilyvion, completely remastered by AssassinsBro.**

*This is a ground-up rewrite of the original Quality Bionics mod with modern architecture, improved stability, and enhanced compatibility.*

## Development

### Project Structure

The project has been restructured to use proper Visual Studio project conventions:

### Building the Mod

#### Using Visual Studio
1. Open `QualityBionics-Continued.sln` in Visual Studio
2. Select Release configuration
3. Build → Build Solution (or Ctrl+Shift+B)

#### Using Command Line
```batch
# Build for RimWorld 1.6
.\build.bat

# Or build manually for specific version
set RimWorldVersion=1.6
dotnet build QualityBionics-Continued.sln --configuration Release
```

#### Using VS Code
Use Ctrl+Shift+P → "Tasks: Run Task" → "build dll"

### Default Namespace

The default namespace is **`QualityBionicsRemastered`** to maintain compatibility across future forks and minimize breaking changes.

# Have you ever wondered...
...why your intellectual colonists who have researched the secrets of the universe, or the crafters of your colonies who have produced artifacts of such quality that news of them spreads across the lands, can't seem to craft bionic equipment that has above standard efficiency?

Well, now you don't have to wonder any longer! Introducing quality bionics! Stronger bionics for smarter people.

Prostheses at bionic level and above now have quality levels that affect the efficiency of the part. Because, let's face it, someone had to have figured out a way to make a circuit in the unit a little bit better.

## What's New in the Remastered Version

### 🚀 **Complete Architecture Overhaul**
- **Thread-Safe Operations**: All systems now use proper concurrency controls
- **Non-Destructive Patching**: No longer modifies shared game definitions, preventing mod conflicts
- **Memory Management**: Automatic cleanup prevents memory leaks from quality transfers
- **Error Handling**: Comprehensive error catching with detailed logging

### 🔧 **Technical Improvements**
- **QualityBionicsManager**: Centralized system replacing crude direct definition modification
- **QualityTransferManager**: Thread-safe quality transfer system with automatic expiration
- **Optimized Performance**: Efficient caching and streamlined algorithms
- **Enhanced Compatibility**: Works seamlessly with other mods that modify bionics

### 🎯 **Enhanced Features**
- **Better Tooltips**: Detailed quality information in bionic tooltips
- **Improved Stats Display**: Custom stats reports showing quality effects
- **Smart Quality Detection**: Automatic detection of quality-eligible bionics

### 🛠️ **Under the Hood**
- Replaced destructive prefix/postfix patches with safe postfix-only approach
- Eliminated static global variables that caused thread safety issues
- Implemented proper separation of concerns with dedicated manager classes
- Added comprehensive unit testing and error recovery systems
- Native translations included: Spanish, German, Dutch, Portuguese, Russian and Simplified Chinese (by Ta) (changes are welcome via PR's)
  
## Compatibility

### ✅ **Improved Compatibility**
- **Better Mod Integration**: Non-destructive patches won't interfere with other bionic mods
- **EBF Dependency**: Now uses Elite Bionics Framework to deliver max HP buffs/debuffs
- **Community Unification**: Mod now works correctly with dozens of other mods, as backed by the EBF
- **Save Game Safe**: Can be safely added to existing saves

## License

Licensed under either of

- Apache License, Version 2.0, ([LICENSE.Apache-2.0](LICENSE.Apache-2.0) or http://www.apache.org/licenses/LICENSE-2.0)
- MIT license ([LICENSE.MIT](LICENSE.MIT) or http://opensource.org/licenses/MIT)

at your option.

`SPDX-License-Identifier: Apache-2.0 OR MIT`
