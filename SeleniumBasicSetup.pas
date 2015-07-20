{
 Code generated at each build by gen-registration.ipy
 This is a subset of SeleniumBasicSetup.iss
}

lib.Guid := '{0277FC34-FD1B-4616-BB19-A9AABCAF2A70}';
lib.FullName := 'Selenium, Version=2.0.2.0, Culture=neutral, PublicKeyToken=d499ab7f7ba4d827';
lib.Description := 'Selenium Type Library';
lib.TypeVersion := '2.0';
lib.PathDll := ExpandConstant('{app}\Selenium.dll');
lib.PathTlb32 := ExpandConstant('{app}\Selenium32.tlb');
lib.PathTlb64 := ExpandConstant('{app}\Selenium64.tlb');
lib.Runtime := 'v2.0.50727';

RegTypeLib(lib);

RegClass(lib, '{0277FC34-FD1B-4616-BB19-0809389E78C4}', 'Selenium.PhantomJSDriver', 'Selenium.PhantomJSDriver');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-14DB1E4916D4}', 'Selenium.FirefoxDriver', 'Selenium.FirefoxDriver');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-5D556733E8C9}', 'Selenium.ChromeDriver', 'Selenium.ChromeDriver');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-5DB46A739EEA}', 'Selenium.List', 'Selenium.List');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-6AAF7EDD33D6}', 'Selenium.Assert', 'Selenium.Assert');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-7D30CBC3F6BB}', 'Selenium.Waiter', 'Selenium.Waiter');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-80B2B91F0D44}', 'Selenium.By', 'Selenium.By');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-9E7F9EF1D002}', 'Selenium.OperaDriver', 'Selenium.OperaDriver');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-A34FCBA29598}', 'Selenium.Utils', 'Selenium.Utils');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-B0C8C528C673}', 'Selenium.Verify', 'Selenium.Verify');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-B719752452AA}', 'Selenium.Table', 'Selenium.Table');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-BE75D14E7B41}', 'Selenium.Keys', 'Selenium.Keys');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-CDCD9EB97FD6}', 'Selenium.PdfFile', 'Selenium.PdfFile');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-CEA7D8FD6954}', 'Selenium.Dictionary', 'Selenium.Dictionary');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-E3CCFFAB4234}', 'Selenium.WebDriver', 'Selenium.WebDriver');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-E9AAFA695FFB}', 'Selenium.Application', 'Selenium.Application');
RegClass(lib, '{0277FC34-FD1B-4616-BB19-EED04A1E4CD1}', 'Selenium.IEDriver', 'Selenium.IEDriver');

RegInterface(lib, '{0277FC34-FD1B-4616-BB19-01D514FE0B1A}', '_Utils', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-0B61E370369D}', '_TableRow', '{00020424-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-0EA52ACB97D1}', '_Assert', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-11660D7615B7}', '_Manage', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-1456C48D8E5C}', '_Dictionary', '{00020424-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-2276E80F5CF7}', '_Cookie', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-384C7E50EFA8}', '_Waiter', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-495CC9DBFB96}', '_Verify', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-4CE442A16502}', '_SelectElement', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-54BA7C175990}', '_Image', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-61DAD6C51012}', '_Keyboard', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-637431245D48}', '_Keys', '{00020424-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-63F894CA99E9}', '_Mouse', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-6E0522EA435E}', '_Application', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-74F5D5680428}', '_Timeouts', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-7C9763568492}', '_WebElements', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-8B145197B76C}', '_WebElement', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-A398E67A519B}', '_DictionaryItem', '{00020424-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-A3DE5685A27E}', '_By', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-B51CB7C5A694}', '_Alert', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-B825A6BF9610}', '_Table', '{00020424-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-BBE48A6D09DB}', '_Actions', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-BE15C121F199}', '_TableElement', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-C539CB44B63F}', '_List', '{00020424-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-C6F450B6EE52}', '_Storage', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-CC6284398AA5}', '_WebDriver', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-D0E30A5D0697}', '_Proxy', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-D5DE929CF018}', '_TouchActions', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-E6E7ED329824}', '_Cookies', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-F2A56C3A68D4}', '_PdfFile', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-FBDA3A91C82B}', '_Window', '{00020420-0000-0000-C000-000000000046}');
RegInterface(lib, '{0277FC34-FD1B-4616-BB19-FFD6FAEF290A}', '_TouchScreen', '{00020420-0000-0000-C000-000000000046}');

RegRecord(lib, '{0277FC34-FD1B-4616-BB19-300DAA508541}', 'Selenium.Strategy');
RegRecord(lib, '{0277FC34-FD1B-4616-BB19-7E2EBB6C82E9}', 'Selenium.Size');
RegRecord(lib, '{0277FC34-FD1B-4616-BB19-ACE280CD7780}', 'Selenium.Point');
RegRecord(lib, '{0277FC34-FD1B-4616-BB19-B342CE81CB2A}', 'Selenium.MouseButton');
RegRecord(lib, '{0277FC34-FD1B-4616-BB19-C724C5135B6E}', 'Selenium.CacheState');
