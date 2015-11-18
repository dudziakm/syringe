$ErrorActionPreference = 'Stop'; # stop on all errors

$packageName = "Syringe"
$toolsDir = $(Split-Path -parent $MyInvocation.MyCommand.Definition)
$serviceExe = "$toolsDir\Syringe.Service\Syringe.Service.exe"
$websiteDir = "$toolsDir\Syringe.Web"
$websiteSetupScript = "$toolsDir\Syringe.Web\bin\iis.ps1"

# Uninstall the service if it exists
if (test-path $serviceDir)
{
	Write-Host "Service found - uninstalling the service."
	& $serviceExe uninstall
}

# Clean up IIS
if (test-path $websiteSetupScript)
{
	Write-Host "Uninstalling IIS site/app pool." -ForegroundColor Green
	Invoke-Expression "$websiteSetupScript $websiteDir true"
}