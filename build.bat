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

:: Build for RimWorld 1.6
echo.
echo Building for RimWorld 1.6...
set RimWorldVersion=1.6
dotnet build --configuration %Configuration% Source\QualityBionicsRemastered\QualityBionicsRemastered.csproj
if %ERRORLEVEL% neq 0 goto :error

echo.
echo Build completed successfully!
exit /b 0

:error
echo.
echo Build failed!
exit /b 1
