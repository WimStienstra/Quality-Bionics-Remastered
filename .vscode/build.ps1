$ErrorActionPreference = 'Stop'

$Target = "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\QualityBionics"

# build dlls
$env:RimWorldVersion = "1.5"
dotnet build 1.5
#dotnet build --configuration Release .vscode
if ($LASTEXITCODE -gt 0) {
    throw "Build failed"
}

# remove mod folder
Remove-Item -Path $Target -Recurse -ErrorAction SilentlyContinue

# copy mod files
New-Item -Path $Target -ItemType Directory -Name 1.5
Copy-Item -Path .\1.5\Assemblies $Target\1.5 -Recurse

Copy-Item -Path About $Target\About -Recurse
Copy-Item -Path Languages $Target\Languages -Recurse

# New-Item -Path $Target -ItemType Directory -Name About
# Copy-Item -Path About\About.xml $Target\About
# Copy-Item -Path About\Manifest.xml $Target\About
# Copy-Item -Path About\Preview.png $Target\About
# Copy-Item -Path About\ModIcon.png $Target\About
# Copy-Item -Path About\PublishedFileId.txt $Target\About

# Copy-Item -Path CHANGELOG.md $Target
# Copy-Item -Path LICENSE $Target
# Copy-Item -Path LICENSE.Apache-2.0 $Target
# Copy-Item -Path LICENSE.MIT $Target
# Copy-Item -Path README.md $Target
