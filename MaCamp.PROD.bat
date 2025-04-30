@echo off
setlocal

:: CONFIGURAÇÕES DO SEU PROJETO
set PROJECT_PATH=.
set KEYSTORE_PATH=MaCamp.keystore
set KEY_ALIAS=MaCamp
set /p PASSWORD=Digite a senha da chave:
set FRAMEWORK=net9.0-android
set CONFIGURATION=Release
set OUTPUT_FOLDER=publish

echo ================================
echo  COMPILANDO APK EM RELEASE...
echo ================================
echo.

:: COMPILAR E ASSINAR
dotnet publish %PROJECT_PATH% ^
    -f %FRAMEWORK% ^
    -c %CONFIGURATION% ^
    -p:AndroidPackageFormat=apk ^
    -p:AndroidKeyStore=true ^
    -p:AndroidSigningKeyAlias=%KEY_ALIAS% ^
    -p:AndroidSigningKeyPass=%PASSWORD% ^
    -p:AndroidSigningStorePass=%PASSWORD% ^
    -p:AndroidSigningKeyStore=%KEYSTORE_PATH%

echo.
echo ================================
echo APK GERADO EM:
echo %PROJECT_PATH%\bin\%CONFIGURATION%\%FRAMEWORK%\%OUTPUT_FOLDER%
echo ================================

pause
endlocal
