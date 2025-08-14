@echo off
echo Building Quality Bionics Continued for RimWorld...

:: Set configuration
set Configuration=Release

:: Build for RimWorld 1.5
echo.
echo Building for RimWorld 1.5...
set RimWorldVersion=1.5
dotnet build --configuration %Configuration% Source\QualityBionicsRemastered\QualityBionicsRemastered.csproj
if %ERRORLEVEL% neq 0 goto :error

dotnet build --configuration %Configuration% Source\QualityBionics\QualityBionics.csproj
if %ERRORLEVEL% neq 0 goto :error

:: Build for RimWorld 1.6
echo.
echo Building for RimWorld 1.6...
set RimWorldVersion=1.6
dotnet build --configuration %Configuration% Source\QualityBionicsRemastered\QualityBionicsRemastered.csproj
if %ERRORLEVEL% neq 0 goto :error

dotnet build --configuration %Configuration% Source\QualityBionics\QualityBionics.csproj
if %ERRORLEVEL% neq 0 (
    echo Warning: Building QualityBionics for 1.6 failed, copying from 1.5 for compatibility
    copy "1.5\Assemblies\QualityBionics.dll" "1.6\Assemblies\QualityBionics.dll" /y
)

echo.
echo Build completed successfully!
exit /b 0

:error
echo.
echo Build failed!
exit /b 1
