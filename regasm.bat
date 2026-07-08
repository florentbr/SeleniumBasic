@echo OFF
if "%1"=="" echo Need a parameter - the directory where the Selenium.dll is resided & exit /B
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe %1\Selenium.dll /codebase

