$ErrorActionPreference = 'Stop'

$Configuration = 'Release'

$Target = "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\QualityBionicsRemastered"

$env:RimWorldSteamWorkshopFolderPath = "..\.deps\refs"
#$env:RimWorldSteamWorkshopFolderPath = "C:\Program Files (x86)\Steam\steamapps\workshop\content\294100"

# build dlls
$env:RimWorldVersion = "1.5"
dotnet build --configuration $Configuration .vscode/mod.csproj
if ($LASTEXITCODE -gt 0) {
    throw "Build failed"
}
dotnet build --configuration $Configuration .vscode/qualitybionics.csproj
if ($LASTEXITCODE -gt 0) {
    throw "Build failed"
}

$env:RimWorldVersion = "1.6"
dotnet build --configuration $Configuration .vscode/mod.csproj
if ($LASTEXITCODE -gt 0) {
    throw "Build failed"
}
# Try to build qualitybionics for 1.6, but if it fails, copy from 1.5 (compatibility)
dotnet build --configuration $Configuration .vscode/qualitybionics.csproj
if ($LASTEXITCODE -gt 0) {
    Write-Warning "Building QualityBionics for 1.6 failed, copying from 1.5 (compatibility)"
    Copy-Item "1.5\Assemblies\QualityBionics.dll" "1.6\Assemblies\QualityBionics.dll" -Force
}

# remove pdbs (for release)
if ($Configuration -eq "Release") {
    Remove-Item -Path .\1.5\Assemblies\QualityBionicsRemastered.pdb -ErrorAction SilentlyContinue
    Remove-Item -Path .\1.5\Assemblies\QualityBionics.pdb -ErrorAction SilentlyContinue
    Remove-Item -Path .\1.6\Assemblies\QualityBionicsRemastered.pdb -ErrorAction SilentlyContinue
    Remove-Item -Path .\1.6\Assemblies\QualityBionics.pdb -ErrorAction SilentlyContinue
}

# remove mod folder
Remove-Item -Path $Target -Recurse -ErrorAction SilentlyContinue

# copy mod files
Copy-Item -Path 1.5 $Target\1.5 -Recurse
Copy-Item -Path 1.6 $Target\1.6 -Recurse

Copy-Item -Path Common $Target\Common -Recurse

Copy-Item -Path LoadFolders.xml $Target

New-Item -Path $Target -ItemType Directory -Name About
Copy-Item -Path About\About.xml $Target\About
Copy-Item -Path About\Manifest.xml $Target\About
Copy-Item -Path About\Preview.png $Target\About
Copy-Item -Path About\ModIcon.png $Target\About

Copy-Item -Path CHANGELOG.md $Target
Copy-Item -Path LICENSE $Target
Copy-Item -Path LICENSE.Apache-2.0 $Target
Copy-Item -Path LICENSE.MIT $Target
Copy-Item -Path README.md $Target
