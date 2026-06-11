@echo off
dotnet build "%~dp0src\WindsOfMagicRestore\WindsOfMagicRestore.csproj" -c Release
if errorlevel 1 exit /b 1

echo.
echo Build OK.
echo Install to game: copy build\WindsOfMagicRestore\ to Modules\WindsOfMagicRestore\
echo Settings UI: enable Mod Configuration Menu v5 (Bannerlord.MBOptionScreen) in launcher, then Mod Options -^> Winds of Magic Restore
exit /b 0
