param([string]$version, [string] $apiKey)

$root = $PSScriptRoot;
$root = $(Split-Path -parent $root)

$nuspecPath = "$root\src\Chocolatey\Syringe\syringe.nuspec"
$installScriptPath = "$root\src\Chocolatey\Syringe\tools\chocolateyinstall.ps1"

write-host "Nuspec path: $nuspecPath"
$contents = [System.IO.File]::ReadAllText($nuspecPath)
$contents = $contents.Replace('$version$', "$version")
[System.IO.File]::WriteAllText($nuspecPath, $contents);

write-host "chocolateyinstall.ps1 path: $installScriptPath"
$contents = [System.IO.File]::ReadAllText($installScriptPath)
$contents = $contents.Replace('{{VERSION}}', "$version")
[System.IO.File]::WriteAllText($installScriptPath, $contents);

choco pack "$root\src\Chocolatey\Syringe\syringe.nuspec"
choco push Syringe.$version.nupkg --source https://www.myget.org/F/syringe/api/v2/package --api-key=$apiKey