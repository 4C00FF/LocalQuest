@echo off
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Requesting admin privileges...
    powershell -Command "Start-Process cmd -ArgumentList '/c cd /d D:\localquest-2021\LocalQuest-master && dotnet run --project LocalQuest' -Verb RunAs"
    exit /b
)
cd /d D:\localquest-2021\LocalQuest-master
dotnet run --project LocalQuest
pause
