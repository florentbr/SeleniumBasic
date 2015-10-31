; AutoIt script example with Selenium
; https://www.autoitscript.com
;

Func Main()
    ; Launch the browser and open an URL
    Dim $driver = ObjCreate("Selenium.FirefoxDriver")
    $driver.Get("https://en.wikipedia.org/wiki/Main_Page")

    ; List all links, remove duplicates and sort them
    Dim $links = $driver.FindElementsByCss("a").Attribute("href")
    $links.Distinct
    $links.Sort

    ; Launch Excel and create a Workbook
    Dim $excel = ObjCreate("Excel.Application")
    $excel.WorkBooks.Add 	; Add a new workbook
    $excel.Visible = 1 		; Let Excel show itself

    ; Write the links in Excel and quit
    $links.ToExcel($excel.ActiveSheet, "Links")
    $driver.Quit
EndFunc

Main()





