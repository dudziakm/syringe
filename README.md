[![Build status](https://ci.appveyor.com/api/projects/status/7l5ooplj6mbdkvfv?svg=true)](https://ci.appveyor.com/project/yetanotherchris/syringe)

# Syringe
Syringe is a .NET automated HTTP testing tool for headless, script-less tests. It is compatable with the webinject HTTP testing tool XML syntax.

## Getting started

 Make sure you have IIS installed. Open a new powershell window and run the `setup.ps1` script, this will:

* Install chocolatey  
* Install nuget command line  
* Build the solution, restoring the nuget packages  
* Create an IIS site
* Create C:\syringe folder with an example file.

If you're on a server, you will need the Microsoft build tools installed:

 choco install microsoft-build-tools -version 14.0.23107.10 -y

You will also need MongoDB installed:

- https://www.mongodb.org/downloads (the choco install doesn't work)
- Create `c:\mongodb\data\db`
- Run `start-mongodb.ps1`
  - Or install it as a service: https://docs.mongodb.org/manual/tutorial/install-mongodb-on-windows/#configure-a-windows-service-for-mongodb

Once this is complete, run `.\run-service.ps1` and go to http://localhost:1980


###Additional VS 2013 setup (for Bal)

*Note: For Visual Studio 2013: install Typescript from http://www.typescriptlang.org/#Download*

If you don't have VS 2013 Service Pack 4, you will need to install the .NET 4.5.2 developer pack for VS 2013 from http://getdotnet.azurewebsites.net/target-dotnet-platforms.html

The download file should be NDP452-KB2901951-x86-x64-DevPack.exe 
