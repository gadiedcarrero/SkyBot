@echo off
echo ========================================
echo   SkyBot Universe - Build Script
echo ========================================
echo.

REM Build en modo Release
echo [1/3] Compilando solucion en modo Release...
dotnet build SkyBot.sln -c Release

if %errorlevel% neq 0 (
    echo.
    echo ERROR: La compilacion fallo
    pause
    exit /b %errorlevel%
)

echo.
echo [2/3] Ejecutando tests...
dotnet test SkyBot.sln -c Release --no-build

if %errorlevel% neq 0 (
    echo.
    echo ADVERTENCIA: Algunos tests fallaron
)

echo.
echo [3/3] Compilacion completada!
echo.
echo ========================================
echo   DLLs generadas en:
echo   src/Robots/SkyCoreAtlas/SkyCoreAtlas.Core/bin/Release/net8.0/
echo   src/Core/SkyCore.Engines/bin/Release/net8.0/
echo   src/Core/SkyCore.Abstractions/bin/Release/net8.0/
echo   src/Core/SkyCore.Common/bin/Release/net8.0/
echo ========================================
echo.
pause
