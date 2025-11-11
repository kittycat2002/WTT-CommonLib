@echo off
setlocal enabledelayedexpansion

REM Load configuration
set CONFIG_FILE=%~dp0publish-config.bat
if exist "!CONFIG_FILE!" (
    call "!CONFIG_FILE!"
) else (
    echo [ERROR] publish-config.bat not found
    echo Run setup-publish.bat first to configure your API keys
    pause
    exit /b 1
)

REM Get solution version as parameter
if "%1"=="" (
    echo [ERROR] Solution version required
    echo Usage: solution-publish-release.bat ^<version^>
    pause
    exit /b 1
)

set VERSION=%1
set BUILDDIR=Build
set OUTPUTZIP=WTT-CommonLib-v!VERSION!.zip

echo.
echo ========================================
echo   [Solution Publish] Release Process
echo ========================================
echo Version:     !VERSION!
echo Build Dir:   !BUILDDIR!
echo Output Zip:  !OUTPUTZIP!
echo.

REM ===== STEP 1: Verify build directory exists =====
if not exist "!BUILDDIR!" (
    echo [ERROR] Build directory not found: !BUILDDIR!
    pause
    exit /b 1
)

echo [1/4] Verified build directory

REM ===== STEP 2: Clean up temporary files =====
echo [2/4] Cleaning up temporary files...

REM Remove all .xml files recursively
for /f "delims=" %%F in ('dir /b /s "!BUILDDIR!\*.xml" 2^>nul') do (
    echo [Cleanup] Removing %%F
    del /f /q "%%F" >nul 2>&1
)

REM Remove all .zip and .7z files recursively
for /f "delims=" %%F in ('dir /b /s "!BUILDDIR!\*.zip" 2^>nul') do (
    del /f /q "%%F" >nul 2>&1
)

for /f "delims=" %%F in ('dir /b /s "!BUILDDIR!\*.7z" 2^>nul') do (
    del /f /q "%%F" >nul 2>&1
)

echo       Cleanup complete

REM ===== STEP 3: Create ZIP =====
echo [3/4] Creating ZIP file...
if exist "!OUTPUTZIP!" (
    echo [WARNING] Removing existing !OUTPUTZIP!
    del "!OUTPUTZIP!"
)

REM Create zip from Build directory
pushd "!BUILDDIR!"
powershell -NoProfile -Command "Add-Type -AssemblyName 'System.IO.Compression.FileSystem'; [System.IO.Compression.ZipFile]::CreateFromDirectory('.', '..\%OUTPUTZIP%')" >nul 2>&1
set ZIPSTATUS=!ERRORLEVEL!
popd

if !ZIPSTATUS! neq 0 (
    echo [ERROR] Failed to create ZIP file
    pause
    exit /b 1
)

if not exist "!OUTPUTZIP!" (
    echo [ERROR] ZIP file was not created
    pause
    exit /b 1
)

echo       Successfully created !OUTPUTZIP!

REM ===== STEP 4: Commit and push =====
echo [4/4] Committing and pushing changes...

git add .
git commit -m "Release v!VERSION!" >nul 2>&1
git push origin >nul 2>&1

if !ERRORLEVEL! neq 0 (
    echo [WARNING] Git commit/push failed - continuing anyway
)

REM ===== STEP 5: Create GitHub Release =====
echo [5/5] Creating GitHub release...

gh release create v!VERSION! "!OUTPUTZIP!" --title "WTT Common Libraries v!VERSION!" --generate-notes

if !ERRORLEVEL! neq 0 (
    echo [ERROR] Failed to create GitHub release
    echo Make sure your code is committed and pushed to GitHub
    pause
    exit /b 1
)

echo       Successfully created GitHub release v!VERSION!

echo.
echo ========================================
echo   [Solution Publish] Release Complete!
echo ========================================
echo.
echo Created:
echo   - Zip File:      !OUTPUTZIP!
echo   - Git Tag:       v!VERSION!
echo   - GitHub Release: v!VERSION!
echo.
pause
endlocal
