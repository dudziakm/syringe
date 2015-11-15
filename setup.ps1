# ===============================================================================
# This script does the following:
# 1. Runs the IIS install tool
# 2. Creates C:\syringe\teamname (default path for tests cases)
# 3. Copies an example test case XML file to that location
# ===============================================================================
$ErrorActionPreference = "Stop"

$configTool = ".\src\Syringe.Web.IisConfig\bin\Debug\Syringe.Web.IisConfig.exe"
$xmlDir     = "C:\syringe\teamname"

Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow
Write-Host "Syringe setup script. " -ForegroundColor DarkYellow
Write-Host "Please read the README file before running this script. " -ForegroundColor DarkYellow
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow

# Install nuget
Write-Host "Installing Nuget." -ForegroundColor Green
choco install nuget.commandline -y

# Build
.\build\build.ps1

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
Write-Host "Create $xmlDir and copying example XML file" -ForegroundColor Green
md $xmlDir -ErrorAction Ignore

# Copy a test file there
Copy-Item -Path "src\Syringe.Tests\Integration\Xml\XmlExamples\Runner\50-cases.xml" -Destination "$xmlDir\50-cases.xml"  -ErrorAction Ignore

# Done
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow
Write-Host "Setup complete." -ForegroundColor Green
Write-host "Now start the REST data service using .\start-service.ps1" -ForegroundColor Cyan
Write-host "- MVC site          : http://localhost:1980/"
Write-Host "- REST api          : http://localhost:8086/swagger/"
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow