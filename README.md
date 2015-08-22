[![Build status](https://ci.appveyor.com/api/projects/status/7l5ooplj6mbdkvfv?svg=true)](https://ci.appveyor.com/project/yetanotherchris/syringe)

# Syringe
Syringe is a .NET version of the webinject testing tool, for automated HTTP testing.

## Dev installation

* Install chocolatey: https://chocolatey.org/
* Visual Studio 2013: install Typescript from http://www.typescriptlang.org/#Download
* Open powershell and run the `setup.ps1` script, this will:
** Creates an IIS site
** Installs Redis
** Creates C:\syringe folder with an example file.
* Run .\run-rest-service.ps1 and go to http://localhost:12345

If Redis doesn't install properly, it can be downloaded at https://github.com/MSOpenTech/redis/releases.

###Additional VS 2013 setup

If you don't have VS 2013 Service Pack 4, you will need to install the .NET 4.5.2 developer pack for VS 2013 from http://getdotnet.azurewebsites.net/target-dotnet-platforms.html

The download file should be NDP452-KB2901951-x86-x64-DevPack.exe 
