# Rebuild the solution
$ErrorActionPreference = "Stop"

$solutionFile      = "Syringe.IIS.sln"
$configuration     = "Debug"
$platform          = "Mixed Platforms"
$msbuild           = "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
$configTool        = ".\src\Syringe.Web.IisConfig\bin\Debug\Syringe.Web.IisConfig.exe"

if (!(Test-Path $msbuild))
{
	$msbuild = "C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
}

# Nuget restoring
Write-Host "Nuget restoring"
nuget restore $solutionFile

# Build the sln file
Write-Host "Building $solutionFile." -ForegroundColor Green
cd $PSScriptRoot
& $msbuild $solutionFile /p:Configuration=$configuration /p:Platform=$platform /target:Build /verbosity:quiet
if ($LastExitCode -ne 0)
{
	throw "Building solution failed."
}
else
{
	Write-Host "Building solution complete."-ForegroundColor Green
}