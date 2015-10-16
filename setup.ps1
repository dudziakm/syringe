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


Write-Host "Make sure you have installed chocolately first https://chocolatey.org " -ForegroundColor DarkYellow
Write-Host "Make sure you have installed.NET 4.5.2 first http://www.microsoft.com/en-gb/download/details.aspx?id=42642 " -ForegroundColor DarkYellow
Write-Host "Visual Studio 2013: make sure you have Typescript installed first: " -ForegroundColor DarkYellow
Write-Host "https://visualstudiogallery.msdn.microsoft.com/b1fff87e-d68b-4266-8bba-46fad76bbf22/file/169854/1/TypeScript_1.5_VS2013.exe" -ForegroundColor DarkYellow

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
Write-host "Now start the REST data service using .\run-rest-service.ps1" -ForegroundColor Cyan
Write-host "- MVC site          : http://localhost:1980/"
Write-Host "- REST api          : http://localhost:8086/swagger/"
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow