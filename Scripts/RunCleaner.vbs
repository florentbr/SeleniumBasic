
' Utility script to 
'  Call driver.Quit on each active session
'  Terminate all the background drivers (chromedriver, iedriver, operadriver, phantomjs)
'  Delete the temporary folder (%TEMP%\Selenium)

Sub Main()
    Call QuitSessions
    Call TerminateDrivers
    Call DeleteTemporaryFolder
    Wscript.Echo "Done!"
End Sub

'Quits all the registered sessions
Sub QuitSessions()
    Err.Clear
    On Error Resume Next
    Do
        GetObject("Selenium.WebDriver").Quit
    Loop Until Err.Number
End Sub

'Terminates all the drivers and all the child processes
Sub TerminateDrivers()
    names = Array("chromedriver.exe", "iedriver.exe", "operadriver.exe", "phantomjs.exe", "edgedriver.exe")
    Set mgt = GetObject("winmgmts:")
    On Error Resume Next
    For Each p In mgt.ExecQuery("Select * from Win32_Process Where Name='" & Join(names, "' Or Name='") & "'")
        For Each cp In mgt.ExecQuery("Select * from Win32_Process Where ParentProcessId=" & p.ProcessId)
            cp.Terminate
        Next
        p.Terminate
    Next
End Sub

'Deletes all the files and folders in "%TEMP%\Selenium"
Sub DeleteTemporaryFolder()
    Set sho = CreateObject("WScript.Shell")
    Set fso = CreateObject("Scripting.FileSystemObject")
    folder = sho.ExpandEnvironmentStrings("%TEMP%\Selenium")
    If fso.FolderExists(folder) Then
        Set folderObj = fso.GetFolder(folder)
        On Error Resume Next
        For Each subfolderObj in folderObj.SubFolders
            subfolderObj.Delete True
        Next
        For Each fileObj in folderObj.Files
            fileObj.Delete True
        Next
        folderObj.Delete True
    End If
End Sub

Call Main
