@echo off
cd /d "%~dp0"

where dotnet >nul 2>nul
if errorlevel 1 (
    echo A .NET SDK nem talalhato. Telepitsd a .NET 10 SDK-t.
    pause
    exit /b 1
)

start "" cmd /c "timeout /t 10 >nul & start http://localhost:5131"
dotnet run --project OnkormanyzatiPortal\OnkormanyzatiPortal --launch-profile http
