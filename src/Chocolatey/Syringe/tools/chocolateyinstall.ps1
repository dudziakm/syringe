$ErrorActionPreference = 'Stop'; # stop on all errors

$packageName = "Syringe"
$toolsDir = $(Split-Path -parent $MyInvocation.MyCommand.Definition)

$version = (wget https://yetanotherchris.blob.core.windows.net/syringe/currentversion.txt).ToString().Trim()
$url = "https://yetanotherchris.blob.core.windows.net/syringe/Syringe-$version.zip"
$url64 = $url

$packageArgs = @{
  packageName   = $packageName
  unzipLocation = $toolsDir
  fileType      = 'EXE' #only one of these: exe, msi, msu
  url           = $url
  url64bit      = $url64
}

# Download
Install-ChocolateyZipPackage $packageName $url $toolsDir

$serviceZip = "$toolsDir\Syringe.Service.$version.0.zip"
$websiteZip = "$toolsDir\Syringe.Web.$version.0.zip"

$serviceDir = "$toolsDir\Syringe.Service"
$websiteDir = "$toolsDir\Syringe.Web"

$serviceExe = "$toolsDir\Syringe.Service\Syringe.Service.exe"
$websiteSetupScript = "$toolsDir\Syringe.Web\bin\setup-iis.ps1"

# Uninstall the service if it exists
if (test-path $serviceDir)
{
	Write-Host "Service found - uninstalling the service."
	& $serviceExe uninstall
}

# Unzip the service + website (overwrites existing files when upgrading)
Get-ChocolateyUnzip  $serviceZip $serviceDir "" $packageName
Get-ChocolateyUnzip  $websiteZip $websiteDir "" $packageName

# Install the service
Write-Host "Installing the Syringe service." -ForegroundColor Green
& $serviceExe install

# Run the website installer
Write-Host "Setting up IIS site." -ForegroundColor Green
Write-Host "$websiteSetupScript $websiteDir"
Invoke-Expression "$websiteSetupScript $websiteDir"