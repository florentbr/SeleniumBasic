using System;
using System.Net;

namespace Selenium.Core {

    interface IDriverService {

        IPEndPoint EndPoint { get; }

        string Uri { get; }

        void Quit();

        void Dispose();

    }

}
