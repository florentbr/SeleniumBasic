using System.Reflection;

namespace Sample {

    class Program {

        static void Main(string[] args) {
            string[] my_args = { 
                Assembly.GetExecutingAssembly().Location,
                "/run:Selenium.Tests"
                // to test an individual fixture, uncomment the following line and add ', Category = "InFocus"' to the fixture
//               ,"/include:InFocus"
            };
            int returnCode = NUnit.ConsoleRunner.Runner.Main(my_args);

            return;
        }
    }

}
