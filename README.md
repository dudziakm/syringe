[![Build status](https://ci.appveyor.com/api/projects/status/l8lcjqu5q0ld1je9?svg=true)](https://ci.appveyor.com/project/yetanotherchris/syringe-4kmo4)
[![Coverage Status](https://coveralls.io/repos/github/TotalJobsGroup/syringe/badge.svg?branch=master)](https://coveralls.io/github/TotalJobsGroup/syringe?branch=master)

# Syringe
Syringe is a .NET automated HTTP testing tool for headless, Javascript-ignorant tests. It is compatable with the webinject HTTP testing tool XML syntax.

## Quickstart

* Make sure you have IIS installed. 

### Pre-requisites

##### Chocolatey

* Install chocolatey
* Install nuget command line : `choco install nuget.commandline`
* Powershell 4+: `choco install powershell4`

##### Mongodb: 

    # Work around for bug in the mongodb Chocolately package
    $env:systemdrive = "C:\ProgramData\chocolatey\lib\mongodata"
    choco install mongodb

##### Install Syringe via myget

*Note: this will configure Syringe on port 80. You should remove any site you have on Port 80, or pass in arguments to use a different port if you don't want to use 80.*

    choco source add -n "myget" -s "https://www.myget.org/F/syringe/api/v2"
    choco install syringe

##### Configure OAuth and start the service

* [Register an Syringe OAuth2 app in Github](https://github.com/settings/developers). The callback url should be http://localhost:1980
* Edit the configuration.json file in the service directory to use the OAuth2 client id/secret.
* Run `.\start-service.ps1` 
* Browse to http://localhost:1980 and login.

#### Building from source

Once you've cloned the repository, run `setup.ps`, this will:

* Build the solution, restoring the nuget packages  
* Create an IIS site
* Create C:\syringe folder with an example file.

Follow the "Configure OAuth" steps above
