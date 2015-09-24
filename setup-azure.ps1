# Install chocolatey
iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))

# Install git
choco install git.install -y -f

# Install MS tools to build
choco install microsoft-build-tools -version 14.0.23107.10 -y

# Install IIS via WebPI
choco install webpi

# Download the source (this can be pull for an update script)
md "c:\ProgramData\syringe" -ErrorAction Ignore
Invoke-Command -ErrorAction Ignore -ScriptBlock { git clone https://github.com/yetanotherchris/syringe.git "c:\ProgramData\syringe" } 

# Run setup
cd "c:\ProgramData\syringe"
.\setup.ps1

# Install the service
& ".\src\Syringe.Service\bin\Debug\Syringe.Service.exe" -install