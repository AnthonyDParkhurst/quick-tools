@echo off

dotnet CheckVcDepends.dll -ignore=msvcrt.dll -ignore=msvcr140d.dll -ignore=msvcp140d.dll -ignore=msvcr140.dll -ignore=msvcp140.dll "-out=D:\Logs\msvcfiles.txt" %* 

pause
