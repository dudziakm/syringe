[![Build status](https://ci.appveyor.com/api/projects/status/l8lcjqu5q0ld1je9?svg=true)](https://ci.appveyor.com/project/yetanotherchris/syringe-4kmo4)
[![Coverage Status](https://coveralls.io/repos/github/TotalJobsGroup/syringe/badge.svg?branch=master)](https://coveralls.io/github/TotalJobsGroup/syringe?branch=master)

# Syringe
Syringe is a .NET automated HTTP testing tool for headless, Javascript-ignorant tests. It is compatable with the webinject HTTP testing tool XML syntax.

## Install the latest version, requires Chocolatey

*Note: this will configure Syringe on port 80. You should remove any site you have on Port 80, or pass in arguments to use a different port if you don't want to use 80.*

    # Work around for a bug with the Chocolatey mongodb package
    $env:systemdrive = "C:\ProgramData\chocolatey\lib\mongodata"
    choco source add -n "myget" -s "https://www.myget.org/F/syringe/api/v2"
    choco install syringe

## Getting started

* Make sure you have IIS installed. 
* Install MongoDB (you will have to do this manually right now, as `choco install mongodb` is broken)
  * Once installed, create `d:\mongodb\data\db`
  * Run `start-mongodb.ps1` or [install MongoDB as a service](https://docs.mongodb.org/manual/tutorial/install-mongodb-on-windows/#configure-a-windows-service-for-mongodb) 
* Install chocolatey  
* Install nuget command line : `choco install nuget.commandline`
* Powershell 4+: `choco install powershell4`
* If you don't have VS installed, you will need the Microsoft build tools installed: `choco install microsoft-build-tools -version 14.0.23107.10 -y`

Then run `setup.ps`, this will:
* Build the solution, restoring the nuget packages  
* Create an IIS site
* Create C:\syringe folder with an example file.

Once this is complete, run `.\start-service.ps1` and go to http://localhost:1980

###Additional VS 2013 setup (for Bal)

*Note: For Visual Studio 2013: install Typescript from http://www.typescriptlang.org/#Download*

If you don't have VS 2013 Service Pack 4, you will need to install the .NET 4.5.2 developer pack for VS 2013 from http://getdotnet.azurewebsites.net/target-dotnet-platforms.html

The download file should be NDP452-KB2901951-x86-x64-DevPack.exe 
