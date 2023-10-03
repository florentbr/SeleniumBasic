using System.Reflection;

// NUnit 2.6.4 docs:
// https://nunit.org/nunitv2/docs/2.6.4/docHome.html

namespace Sample {

    class Program {

        static void Main(string[] args) {
            // Run arguments: https://nunit.org/nunitv2/docs/2.6.4/consoleCommandLine.html
            string[] my_args = { 
                Assembly.GetExecutingAssembly().Location
                // to run only particular testfixture (class) or testcase (function), list them (comma separated) in the /run argument, like:
                // ,"/run:Selenium.Tests.TS_Scraping.ShouldScrapTextFromTable"
                // to include only particular browser(s) to the testing, list them (comma separated) in the /include argument, like:
               ,"/include:Edge"
            };
            int returnCode = NUnit.ConsoleRunner.Runner.Main(my_args);

            return;
        }
    }

}
