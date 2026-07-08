using Selenium.Zip;
using System;
using System.IO;
using System.ComponentModel;
using A = NUnit.Framework.Assert;
using Test = NUnit.Framework.TestAttribute;
using TestFixture = NUnit.Framework.TestFixtureAttribute;

namespace Selenium.Tests {

    [TestFixture]
    class TS_Zip {

        private void CreateTextFile(string path, string text){
            using (StreamWriter sw = File.CreateText(path))
                sw.Write(text);
        }

        [Test]
        public void ShouldZipAndUnzipNoCompression() {
            ShouldZipAndUnzipWithoutLose(false);
        }

        [Test]
        public void ShouldZipAndUnzipWithCompression() {
            ShouldZipAndUnzipWithoutLose(true);
        }

        private void ShouldZipAndUnzipWithoutLose(bool compress){

            string testfolder = Path.Combine(System.IO.Path.GetTempPath(), DateTime.Now.Ticks.ToString());
            Directory.CreateDirectory(testfolder);

            string path1 = Path.Combine(testfolder, "text1.txt");
            string path2 = Path.Combine(testfolder, "text2.txt");

            string text1 = "";
            string text2 = "START data\r\n ééúóáEN\u0247D";

            CreateTextFile(path1, text1);
            CreateTextFile(path2, text2);

            string zippath = Path.Combine(testfolder, "archive.zip");
            string extractdir = Path.Combine(testfolder, "extract");
            string path11 = Path.Combine(extractdir, "text1.txt");
            string path22 = Path.Combine(extractdir, "text2.txt");

            try {
                using (var zip = new ZipFile(compress)) {
                    zip.AddFile(path1);
                    zip.AddFile(path2);
                    zip.SaveAs(zippath);
                }

                ZipFile.ExtractAll(zippath, extractdir);

                A.True(Directory.Exists(extractdir));
                A.True(File.Exists(path11));
                A.True(File.Exists(path22));

                var text11 = File.ReadAllText(path11);
                A.AreEqual(text1, text11);

                var text22 = File.ReadAllText(path22);
                A.AreEqual(text2, text22);
            } finally {
                Directory.Delete(testfolder, true);
            }
        }

    }

}
