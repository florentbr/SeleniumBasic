@echo off

echo Clean registry ...

set f=%TEMP%\reglist.txt
IF EXIST %f% del /F %f%

REG QUERY HKLM\Software\Classes /f "Selenium." /k /s >> %f%
REG QUERY HKCU\Software\Classes /f "Selenium." /k /s >> %f%
REG QUERY HKLM\SOFTWARE\Classes /f "{0277FC34-FD1B-4616-BB19" /k /s>> %f%
REG QUERY HKCU\SOFTWARE\Classes /f "{0277FC34-FD1B-4616-BB19" /k /s>> %f%

setlocal enableextensions enabledelayedexpansion
for /f "tokens=*" %%a in (%f%) do (
    Set line=%%a
    if "!line:~,5!"=="HKEY_" (
        echo !line!
        REG DELETE !line! /f
    )
)
endlocal


echo Done.
pause