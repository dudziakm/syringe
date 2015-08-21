[![Build status](https://ci.appveyor.com/api/projects/status/7l5ooplj6mbdkvfv?svg=true)](https://ci.appveyor.com/project/yetanotherchris/syringe)

# Syringe
Syringe is a .NET version of the webinject testing tool, for automated HTTP testing.

## Dev installation

Make sure you have Redis and Typescript installed:

* Install chocolatey: https://chocolatey.org/
* Open a new Powershell prompt: `choco install redis -y` (alternatively download from https://github.com/MSOpenTech/redis/releases)
* Install Typescript from http://www.typescriptlang.org/#Download (for Visual Studio 2013)

Open powershell and run the setup.ps1 script to install the IIS site, and then run .\run-rest-service.ps1

Additional VS 2013 setup

If you have VS 2015 you will not have the following problems:

Intall the .net 4.5.2 developer pack for VS 2015 on the following link http://getdotnet.azurewebsites.net/target-dotnet-platforms.html
the download file should be NDP452-KB2901951-x86-x64-DevPack.exe 