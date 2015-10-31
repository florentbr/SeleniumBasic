; AutoIt script template with Selenium
; https://www.autoitscript.com
;

Func Main()
    ; Launch the browser and open an URL
    Dim $driver = ObjCreate("Selenium.FirefoxDriver")
    $driver.Get("https://en.wikipedia.org/wiki/Main_Page")

    $driver.Wait(2000)

    $driver.Quit
EndFunc

Main()