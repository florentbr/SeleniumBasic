; AutoIt script example
; https://www.autoitscript.com
;


; Launch the browser and open an URL
Local $driver = ObjCreate("Selenium.FirefoxDriver")
$driver.Get("https://en.wikipedia.org/wiki/Main_Page")

; List all links, remove duplicates and sort them
Local $links = $driver.FindElementsByCss("a").Attribute("href")
$links.Distinct
$links.Sort

; Launch Excel and create a Workbook
Local $excel = ObjCreate("Excel.Application")
$excel.WorkBooks.Add 	; Add a new workbook
$excel.Visible = 1 		; Let Excel show itself

; Write the links in Excel and quit
$links.ToExcel($excel.ActiveWorkBook.ActiveSheet, "Links")
$driver.Quit


