using System.Reflection;

namespace Sample {

    class Program {

        static void Main(string[] args) {
            string[] my_args = { 
                Assembly.GetExecutingAssembly().Location,
                "/run:Selenium.Tests"
            };
            int returnCode = NUnit.ConsoleRunner.Runner.Main(my_args);

            return;
        }
    }

}
