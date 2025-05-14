rem @echo off

set pathName=%~dp1
set baseName=%~n1

rem msiexec.exe /quiet /a "%~f1" TARGETDIR="%pathName%%baseName%AdminInstall"
msiexec.exe /qb+ /a "%~f1" TARGETDIR="%pathName%%baseName%AdminInstall"

@echo off

IF %ERRORLEVEL% EQU 0 goto Success

echo ***************************************************************
echo ERROR: Admin install failed... Perhaps a problem with MAX PATH!
echo ***************************************************************

pause
EXIT /B 1

:Success
