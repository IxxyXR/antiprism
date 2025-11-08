@echo off
REM Build script for Antiprism Unity Plugin on Windows
REM This handles both CMake configuration and building

echo ========================================
echo Antiprism Unity Plugin Build Script
echo ========================================
echo.

REM Check if build directory exists
if not exist build (
    echo Creating build directory...
    mkdir build
)

cd build

echo.
echo [1/2] Configuring CMake...
echo ========================================
cmake .. -G "Visual Studio 17 2022" -A x64 -DCMAKE_BUILD_TYPE=Release

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: CMake configuration failed!
    cd ..
    exit /b %ERRORLEVEL%
)

echo.
echo [2/2] Building Release configuration...
echo ========================================
cmake --build . --config Release

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Build failed!
    cd ..
    exit /b %ERRORLEVEL%
)

echo.
echo ========================================
echo Build completed successfully!
echo ========================================
echo.
echo Output: build\Release\antiprism.dll
echo.

cd ..
