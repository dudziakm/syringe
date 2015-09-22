[![Build status](https://ci.appveyor.com/api/projects/status/7l5ooplj6mbdkvfv?svg=true)](https://ci.appveyor.com/project/yetanotherchris/syringe)

# Syringe
Syringe is a .NET version of the webinject testing tool, for automated HTTP testing.

## Dev installation

Note: For Visual Studio 2013: install Typescript from http://www.typescriptlang.org/#Download
Open powershell and run the `setup.ps1` script, this will:

* Install chocolatey  
* Install nuget command line  
* Build the solution, restoring the nuget packages  
* Create an IIS site
* Create C:\syringe folder with an example file.

Once this is complete, run `.\run-rest-service.ps1` and go to http://localhost:8085

###Additional VS 2013 setup (for Bal)

If you don't have VS 2013 Service Pack 4, you will need to install the .NET 4.5.2 developer pack for VS 2013 from http://getdotnet.azurewebsites.net/target-dotnet-platforms.html

The download file should be NDP452-KB2901951-x86-x64-DevPack.exe 
