
"""Basic tests to ensure all the brosers are launchable with the COM server
"""

import sys, os, unittest, threading, time, BaseHTTPServer
from win32com import client  #http://sourceforge.net/projects/pywin32/files/pywin32/

SERVER_ADDRESS = ('127.0.0.1', 9589)

class TestStringMethods(unittest.TestCase):
    
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


def CreateObject(appid):
    return client.Dispatch(appid)

class HTTPServerHandler(BaseHTTPServer.BaseHTTPRequestHandler):
    def do_GET(s):
        s.send_response(200)
        s.send_header('Content-type', 'text/html')
        s.end_headers()
        s.wfile.write('<html>')
        s.wfile.write('<head><title>Title</title></head>')
        s.wfile.write('<body><a id="link">Test page</a></body>')
        s.wfile.write('</html>')
    
    def log_message(self, format, *args):
        return

class HTTPServer(threading.Thread):
    def __init__(self, address, handler):
        threading.Thread.__init__(self)
        self.server_address = address
        self.handler = handler
    
    def run(self):
        self.httpd = BaseHTTPServer.HTTPServer(self.server_address, self.handler)
        self.httpd.serve_forever()
        
    def stop(self):
        self.httpd.shutdown()

if __name__ == '__main__':
    print __doc__
    print "Start tests ..."
    server = HTTPServer(SERVER_ADDRESS, HTTPServerHandler)
    try:
        server.start()
        time.sleep(0.1)
        unittest.main()
    finally:
        server.stop()
