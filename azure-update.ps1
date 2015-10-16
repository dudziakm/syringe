# Remove the service
& ".\src\Syringe.Service\bin\Debug\Syringe.Service.exe" uninstall

# Pull the source
git pull

# Re-build
.\build.ps1

# Re-install the service
& ".\src\Syringe.Service\bin\Debug\Syringe.Service.exe" install start --localsystem