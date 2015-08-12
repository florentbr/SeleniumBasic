
' Utility script to launch Chrome with a listening port
' Usage:
'   Set driver = CreateObject("Selenium.ChromeDriver")
'   driver.SetCapability "debuggerAddress", "127.0.0.1:9222"
'   driver.Get "https://www.google.co.uk"

Const LISTENNING_PORT = 9222

Sub Main()
    Set shell = WScript.CreateObject("WScript.Shell")
    shell.Run "chrome.exe --remote-debugging-port=" & LISTENNING_PORT
End Sub

Call Main
