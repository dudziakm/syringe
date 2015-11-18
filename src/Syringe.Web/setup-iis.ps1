#=================================================================
# Add a Syringe app pool and website, removing any existing ones.
#=================================================================
param ([string] $websitePath = $PSScriptRoot)

$appPoolName = "Syringe"
$websiteName = "Syringe"
$websitePort = 1980
$websitePath = $PSScriptRoot

Import-Module WebAdministration

function Test-WebAppPool($Name) {
    return Test-Path "IIS:\AppPools\$Name"
}

function Test-Website($Name) {
    return Test-Path "IIS:\Sites\$Name"
}

if (Test-WebAppPool $appPoolName)
{
    Write-Host "  Removing app pool $appPoolName"
    Remove-WebAppPool -Name $appPoolName -WarningAction Ignore
}

if (Test-Website $websiteName)
{
    Write-Host "  Removing website $websiteName"
    Remove-Website -Name $websiteName -WarningAction Ignore
}

#=================================================================
# Add the app pool first
#=================================================================
Write-Host "  Adding app pool $appPoolName (v4, localservice)"
New-WebAppPool -Name $appPoolName -Force | Out-Null
Set-ItemProperty "IIS:\AppPools\$appPoolName" managedRuntimeVersion v4.0
Set-ItemProperty "IIS:\AppPools\$appPoolName" managedPipelineMode Integrated
Set-ItemProperty "IIS:\AppPools\$appPoolName" processModel -value @{userName="";password="";identitytype=1}
Set-ItemProperty "IIS:\AppPools\$appPoolName" processModel.idleTimeout -value ([TimeSpan]::FromMinutes(0))
Set-ItemProperty "IIS:\AppPools\$appPoolName" processModel.pingingEnabled -value true #disable for debuging

#=================================================================
# Syringe website
#=================================================================
Write-Host "  Adding website $websiteName (id:$websitePort, port: $websitePort, path: $websitePath)"
New-Website -Name $websiteName -Id $websitePort -Port $websitePort -PhysicalPath .\ -ApplicationPool $appPoolName -Force  | Out-Null