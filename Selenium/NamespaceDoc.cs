using System.Runtime.CompilerServices;

namespace Selenium {
    /// \mainpage
    /// <summary>
    /// Selenium namespace of classes.
    /// </summary>
    /// <remarks>
    /// There are two modes of operation: modern and legacy.
    /// gecko driver works only in the modern mode since it only supports the <see href="https://w3c.github.io/webdriver/#endpoints">modern endpoints</see>.
    /// chrome and edge drivers should also support the modern mode, but since they also support the legacy endpoints
    /// they operate in the lagacy mode for compatibility reasons.
    /// </remarks>
    /// <example>
    /// <code lang="VB">	
    /// Public Sub Script()
    ///   Dim driver As New ChromeDriver
    ///   driver.Get "http://www.google.com"
    ///   ...
    ///   driver.Quit
    /// End Sub
    /// </code>
    /// 
    /// <code lang="vbs">	
    ///     Dim driver
    ///     Set driver = CreateObject("Selenium.GeckoDriver")
    ///     driver.Get "http://www.google.com"
    ///     ..........
    ///     driver.Quit
    /// </code>
    /// </example>
    [CompilerGeneratedAttribute()]
    public class NamespaceDoc { }
}
