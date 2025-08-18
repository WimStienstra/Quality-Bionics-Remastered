# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.3.3] - 2025-08-18
- **Translations**: Added Simplified Chinese translation of Ta. And added other translations with the use of AI, checked German, Dutch and Spanish. I think they're okay, maybe there are some better wording possibilities, so PR's are welcome.

## [0.3.2] - 2025-08-17
- **BREAKING**: Integrated the Pull Request of Vectorial1024 to fully integrate with Elite Bionics Framework + changes regarding this
- **Compatibility**: Dropped compatibility with 1.5

## [0.3.1] - 2025-08-11
### Changed
- **Architecture**: Restructuring of whole mod

## [0.3.0] - 2025-07-18 - REMASTERED VERSION

### Changed
- **BREAKING**: Complete mod rewrite and rename to "Quality Bionics Remastered"
- **Architecture**: Ground-up rewrite with modern thread-safe architecture
- **Compatibility**: Non-destructive patching for better mod compatibility
- **Performance**: Improved performance and stability with centralized management
- **Namespaces**: All code moved from `QualityBionicsContinued` to `QualityBionicsRemastered`
- **Package ID**: Changed to `assassinsbro.qualitybionicsremastered`

### Added
- QualityBionicsManager: Centralized thread-safe management system
- QualityTransferManager: Thread-safe quality transfer system with automatic cleanup
- Enhanced error handling and comprehensive logging throughout
- Improved tooltips showing detailed quality information
- Custom stats reports for quality bionics

### Fixed
- Thread safety issues with concurrent collections
- Memory leaks from uncleaned static variables
- Mod conflicts from destructive definition modifications
- Performance issues from inefficient LINQ operations

### Technical
- Replaced destructive prefix/postfix patches with safe postfix-only approach
- Eliminated direct modification of shared game definitions
- Implemented proper concurrent collections with expiration handling
- Added comprehensive error recovery and logging systems

## [0.2.1] - 2024-05-20

### Fixed

-   Use `__args` in `RecipeWorker_ApplyOnPawn` patch instead of named parameters so that subclasses renaming parameter names won't matter.

## [0.2.0] - 2024-05-18

### Fixed

-   EBF compatibility should be back after adding a compatibility layer so it can keep pretending the mod is like it was in 1.4.

## [0.1.0] - 2024-04-30

### Added

-   Ported [original mod](https://github.com/SirRebelRabbit/Quality-Bionics) from 1.4 to 1.5

[Unreleased]: https://github.com/ilyvion/Quality-Bionics-Continued/compare/v0.2.1...HEAD
[0.2.1]: https://github.com/ilyvion/Quality-Bionics-Continued/compare/v0.2.0...v0.2.1
[0.2.0]: https://github.com/ilyvion/Quality-Bionics-Continued/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/ilyvion/Quality-Bionics-Continued/releases/tag/v0.1.0
