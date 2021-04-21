@echo off 

if "%1" == "--no-build" (
    dotnet run --project ./Thermometer/Thermometer.csproj --no-build
) else (
    dotnet run --project ./Thermometer/Thermometer.csproj 
)
