using A = NUnit.Framework.Assert;
using Test = NUnit.Framework.TestAttribute;
using TestFixture = NUnit.Framework.TestFixtureAttribute;

namespace Selenium.Tests {

    [TestFixture]
    class TS_PDF {

        [Test]
        public void ShouldCreatePDF() {
            string tempPath = System.IO.Path.GetTempPath() + @"\my-capture.pdf";
            if (System.IO.File.Exists(tempPath))
                System.IO.File.Delete(tempPath);

            var pdf = new PdfFile();

            //Define the PDF page size and margins

            pdf.SetPageSize(210, 297, "mm");
            pdf.SetMargins(10, 10, 10, 15, "mm");

            //Add a title and informations to the PDF
            pdf.AddTextCenter(text: "Selenium search result", size: 20, bold: true);
            pdf.AddTitle("Title A");
            pdf.AddText(text: "Description = Search for Eiffel tower", size: 9, color: "DarkBlue");
            pdf.AddLink("http://www.test.com", "Test link");

            //Add a text paragraph to the PDF file
            string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam sit amet libero arcu, et molestie purus. Ut in sem lacus, sit amet rhoncus erat. In aliquet arcu at nunc porta sollicitudin. Cras ante nisl, hendrerit quis bibendum quis, egestas vitae mi. Donec ac felis at eros placerat iaculis. Nam quam sapien, scelerisque vel bibendum et, mollis sit amet augue. Nullam egestas, lectus ut laoreet vulputate, neque quam vestibulum sapien, ut vehicula nunc metus et nulla. Curabitur ac lorem augue. Nullam quis justo eu arcu volutpat ultrices ac at orci.";
            pdf.AddText(text: text, size: 10);

            pdf.AddPage();

            //Take a screenschot and add it to the PDF file
            using (var img = new Utils().TakeScreenShot()) {
                pdf.AddBookmark("Bookmark");
                pdf.AddTitle("Capture A");
                pdf.AddImage(img, false);
            }

            pdf.SaveAs(tempPath);

            A.True(System.IO.File.Exists(tempPath));
            A.Greater(new System.IO.FileInfo(tempPath).Length, 25000);

        }

    }

}
