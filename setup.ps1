#
# This script does the following:
# 1. Builds the solution, including the IIS install tool
# 2. Runs the IIS install tool
# 3. Creates C:\syringe\teamname (default path for tests cases)
# 4. Copies an example test case XML file to that location
#
$ErrorActionPreference = "Stop"

$solutionFile      = "Syringe.IIS.sln"
$configuration     = "Debug"
$platform          = "Mixed Platforms"
$msbuild           = "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
$configTool        = ".\src\Syringe.Web.IisConfig\bin\Debug\Syringe.Web.IisConfig.exe"

Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow
Write-Host "Syringe setup script. " -ForegroundColor DarkYellow
Write-Host "Please read the README file before running this script. " -ForegroundColor DarkYellow
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow

# Install nuget to restore
Write-Host "Installing Nuget." -ForegroundColor Green
choco install nuget.commandline

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

# Run IISConfig tool
Write-Host "Running the IIS setup tool." -ForegroundColor Green
& $configTool
if ($LastExitCode -ne 0)
{
	Write-Host "IISConfig setup failed."-ForegroundColor Red
	exit 1
}
else
{
	Write-Host "IIS Setup complete." -ForegroundColor Green
}

# Create c:\syringe\teamname
Write-Host "Create c:\Syringe\teamname and copying example XML file" -ForegroundColor Green
md c:\syringe\teamname -ErrorAction Ignore

# Copy a test file there
Copy-Item -Path "src\Syringe.Tests\Integration\Xml\XmlExamples\Runner\50-cases.xml" -Destination "C:\syringe\teamname\50-cases.xml"  -ErrorAction Ignore

# Done
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow
Write-Host "Setup complete." -ForegroundColor Green
Write-host "Now start the REST data service using .\start-service.ps1" -ForegroundColor Cyan
Write-host "- MVC site          : http://localhost:1980/"
Write-Host "- REST api          : http://localhost:8086/swagger/"
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow