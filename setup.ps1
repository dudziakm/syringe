# ===============================================================================
#
# Syringe developer setup script.
#
# This script does the following:
# 1. Runs the IIS setup script
# 2. Creates C:\syringe\teamname (default path for tests cases)
# 3. Copies an example test case XML file to that location
# ===============================================================================
$ErrorActionPreference = "Stop"
$xmlDir     = "C:\syringe\teamname"
$websiteDir = Resolve-Path ".\src\Syringe.Web\"
$mongoDataDir = "D:"#$env:ChocolateyInstall +"\lib\mongodata"

Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow
Write-Host "Syringe setup script. " -ForegroundColor DarkYellow
Write-Host "Please read the README file before running this script. " -ForegroundColor DarkYellow
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow

# Install nuget
Write-Host "Installing Nuget." -ForegroundColor Green
choco install nuget.commandline -y

# Install MongoDB, hack $env:sysdrive as the installer relies on it
$oldSysDrive = $env:systemdrive
$env:systemdrive = $mongoDataDir
choco install mongodb -y --force
$env:systemdrive = $oldSysDrive

# Build
Write-Host "Building solution." -ForegroundColor Green
.\build\build.ps1 "Debug"

# Setup IIS
Write-Host "Creating IIS app pool and site." -ForegroundColor Green
.\src\Syringe.Web\bin\iis.ps1 $websiteDir

# Create c:\syringe\teamname
Write-Host "Create $xmlDir and copying example XML file" -ForegroundColor Green
md $xmlDir -ErrorAction Ignore

# Copy a test file there
Copy-Item -Path "src\Syringe.Tests\Integration\Xml\XmlExamples\Runner\50-cases.xml" -Destination "$xmlDir\50-cases.xml"  -ErrorAction Ignore

# Done
Write-Host ""
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow
Write-Host "Setup complete." -ForegroundColor Green
Write-host "Now start the REST data service using tools\start-service.ps1" -ForegroundColor Cyan
Write-host "Don't forget to start MongoDB too (if it's not a service) - tools\start-mongodb.ps1" -ForegroundColor Cyan
Write-Host ""
Write-host "- MVC site          : http://localhost:1980/"
Write-Host "- REST api          : http://localhost:1981/swagger/"
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow
