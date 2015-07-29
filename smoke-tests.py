
"""Basic tests to ensure all the browsers are launchable with the COM server
"""

import sys, os, unittest, threading, time, multiprocessing, BaseHTTPServer
from win32com import client  #http://sourceforge.net/projects/pywin32/files/pywin32/

SERVER_ADDRESS = ('127.0.0.1', 9393)
SERVER_PAGE = """
<html>
<head>
    <title>Title</title>
</head>
<body>
    <a id="link">Test page</a>
</body>
</html>
"""

class Suite(unittest.TestCase):
    
    def test_list(self):
        lst = CreateObject("Selenium.List")
        for i in range(0, 10):
            lst.add(i)
        self.assertEqual(10, lst.Count)
    
    def test_firefox(self):
        self.assert_browser_display_page("Selenium.FirefoxDriver")         
    
    def test_iedriver(self):
        self.assert_browser_display_page("Selenium.IEDriver")
    
    def test_chrome(self):
        self.assert_browser_display_page("Selenium.ChromeDriver")
    
    def test_opera(self):
        self.assert_browser_display_page("Selenium.OperaDriver")
    
    def test_phantomjs(self):
        self.assert_browser_display_page("Selenium.PhantomJSDriver")
    
    def assert_browser_display_page(self, progid):
        driver = CreateObject(progid)
        try:
            driver.get("http://%s:%s" % SERVER_ADDRESS)
            txt = driver.FindElementById('link').Text
            self.assertEqual("Test page", txt)
        finally:
            driver.quit



def CreateObject(progid):
    return client.Dispatch(progid)

def RunHTTPServer():
    server = BaseHTTPServer.HTTPServer(SERVER_ADDRESS, HTTPServerHandler)
    server.serve_forever()

class HTTPServerHandler(BaseHTTPServer.BaseHTTPRequestHandler):
    
    def handle(self):
        try:
            return BaseHTTPServer.BaseHTTPRequestHandler.handle(self)
        except: return
    
    def log_message(self, format, *args):
        return
    
    def do_GET(s):
        s.send_response(200)
        s.send_header('Content-type', 'text/html')
        s.end_headers()
        s.wfile.write(SERVER_PAGE)


if __name__ == '__main__':
    print __doc__
    print "Start tests ...\n"
    server = multiprocessing.Process(target=RunHTTPServer)
    server.start()
    try:
        unittest.main()
    except SystemExit: pass