@echo off
setlocal enabledelayedexpansion

echo [NuGet] PackageId: %1
echo [NuGet] Version: %2

set PACKAGEID=%1
set VERSION=%2

if "!PACKAGEID!"=="" (
    echo [ERROR] PackageId missing
    exit /b 1
)

if "!VERSION!"=="" (
    echo [ERROR] Version missing
    exit /b 1
)

cd /d "%~dp0"
echo [NuGet] Current directory: !CD!

REM Find solution root
set SOLUTIONDIR=
for /f "delims=" %%D in ('cd') do set CURRENTDIR=%%D

:find_solution
if exist "%CURRENTDIR%\*.sln" (
    set SOLUTIONDIR=!CURRENTDIR!
    goto :found_solution
)
for %%A in ("!CURRENTDIR!\..") do set CURRENTDIR=%%~fA
if "!CURRENTDIR!"=="!CURRENTDIR:~0,3!" exit /b 1
goto :find_solution

:found_solution
echo [NuGet] Found solution directory: !SOLUTIONDIR!

set CONFIG_FILE=!SOLUTIONDIR!\publish-config.bat
if not exist "!CONFIG_FILE!" (
    echo [WARNING] publish-config.bat not found - skipping NuGet publish
    exit /b 0
)

call "!CONFIG_FILE!"

if "!NuGetAPIKey!"=="" (
    echo [WARNING] NuGetAPIKey not configured - skipping NuGet publish
    exit /b 0
)

echo [NuGet] Creating NuGet package for !PACKAGEID! v!VERSION!...
dotnet pack -c Publish --no-build
echo [NuGet] Pack exit code: !ERRORLEVEL!

echo [NuGet] Attempting to locate .nupkg file for version !VERSION!...

REM Search for the specific version's nupkg file
set NUPKG=

for /f "delims=" %%F in ('dir /b /s bin\*.nupkg 2^>nul') do (
    echo %%F | findstr /i "!VERSION!" >nul 2>&1
    if !ERRORLEVEL! equ 0 (
        set NUPKG=%%F
        echo [NuGet] Found matching version: !NUPKG!
    )
)

if "!NUPKG!"=="" (
    echo [WARNING] Could not find .nupkg file for version !VERSION!
    echo [NuGet] Debug - Looking for: *!VERSION!*.nupkg
    exit /b 0
)

echo [NuGet] Publishing !PACKAGEID! v!VERSION! to NuGet...
echo [NuGet] Package path: !NUPKG!

REM Push to NuGet
dotnet nuget push "!NUPKG!" --api-key !NuGetAPIKey! --source https://api.nuget.org/v3/index.json

if !ERRORLEVEL! neq 0 (
    echo [WARNING] Failed to publish to NuGet ^(exit code !ERRORLEVEL!^)
    exit /b 0
)

echo [NuGet] Successfully published !PACKAGEID! v!VERSION!
endlocal
exit /b 0
