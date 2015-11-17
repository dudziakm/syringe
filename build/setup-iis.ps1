#https://technet.microsoft.com/en-us/library/ee790599.aspx
#http://stackoverflow.com/questions/4229082/how-do-i-set-the-net-framework-version-when-using-new-webapppool
#https://msdn.microsoft.com/en-us/library/aa347554(v=VS.90).aspx
#https://msdn.microsoft.com/en-us/library/bb386459(v=vs.90).aspx

Remove-WebAppPool -Name "Syringe" -WarningAction Ignore
Remove-Website -Name "Syringe"  -WarningAction Ignore

$appPoolName = "Syringe"
New-WebAppPool -Name $appPoolName
Set-ItemProperty IIS:\AppPools\$appPoolName managedRuntimeVersion v4.0
Set-ItemProperty IIS:\AppPools\$appPoolName managedPipelineMode Integrated
Set-ItemProperty IIS:\AppPools\$appPoolName processModel -value @{userName="";password="";identitytype=1}
Set-ItemProperty IIS:\AppPools\$appPoolName processModel.idleTimeout -value ([TimeSpan]::FromMinutes(0))
Set-ItemProperty IIS:\AppPools\$appPoolName processModel.pingingEnabled -value true

#New-Website [-Name] <string> [-Id <uint32>] [-Port <uint32>] [-IPAddress <string>] [-SslFlags <int>] [-HostHeader <string>] [-PhysicalPath <string>]
# [-ApplicationPool <string>] [-Ssl] [-Force]  [<CommonParameters>]
New-Website -Name "Syringe" -Id 1980 -Port 1980 -PhysicalPath .\ -ApplicationPool Syringe