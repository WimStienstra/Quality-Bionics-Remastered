$ErrorActionPreference = 'Stop'

$Configuration = 'Release'

$Target = "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\QualityBionicsContinued"

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

# remove pdbs (for release)
if ($Configuration -eq "Release") {
    Remove-Item -Path .\1.5\Assemblies\QualityBionicsContinued.pdb -ErrorAction SilentlyContinue
    Remove-Item -Path .\1.5\Assemblies\QualityBionics.pdb -ErrorAction SilentlyContinue
}

# remove mod folder
Remove-Item -Path $Target -Recurse -ErrorAction SilentlyContinue

# copy mod files
Copy-Item -Path 1.5 $Target\1.5 -Recurse

Copy-Item -Path Common $Target\Common -Recurse

New-Item -Path $Target -ItemType Directory -Name About
Copy-Item -Path About\About.xml $Target\About
Copy-Item -Path About\Manifest.xml $Target\About
Copy-Item -Path About\Preview.png $Target\About
Copy-Item -Path About\ModIcon.png $Target\About
Copy-Item -Path About\PublishedFileId.txt $Target\About

Copy-Item -Path CHANGELOG.md $Target
Copy-Item -Path LICENSE $Target
Copy-Item -Path LICENSE.Apache-2.0 $Target
Copy-Item -Path LICENSE.MIT $Target
Copy-Item -Path README.md $Target
