@echo off
rem Builds the API once and starts Party A and Party B, each in its own window.

set PROJECT=%~dp0src\DocumentExchange.Api

dotnet build "%PROJECT%" || exit /b 1

start "Party A" cmd /k dotnet run --no-build --project "%PROJECT%" --launch-profile PartyA
start "Party B" cmd /k dotnet run --no-build --project "%PROJECT%" --launch-profile PartyB
