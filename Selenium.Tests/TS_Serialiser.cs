using Selenium.Serializer;
using Selenium.Zip;
using System;
using System.Drawing.Imaging;
using System.IO;
using A = NUnit.Framework.Assert;
using CA = NUnit.Framework.CollectionAssert;
using Test = NUnit.Framework.TestAttribute;
using TestCase = NUnit.Framework.TestCaseAttribute;
using TestFixture = NUnit.Framework.TestFixtureAttribute;

namespace Selenium.Tests {

    [TestFixture]
    public class TS_Serializer {

        [TestCase("string         ", "\"abcd\"", "\"abcd\"")]
        [TestCase("doubles quotes ", "abcdefg \" ' 0123 \0 \u0234 ", "abcdefg \" ' 0123 \0 \u0234 ")]
        [TestCase("unicode        ", "\u0019\u001F\u001a", "\u0019\u001F\u001A")]
        [TestCase("escape         ", "\b\r\\\\\\\f\n", "\b\r\\\\\\\f\n")]
        [TestCase("array          ", "[1,2,3,4,5]", "[1,2,3,4,5]")]
        [TestCase("dictionary     ", @"{""a"":[1,2],""b"":675}", @"{""a"":[1,2],""b"":675}")]
        [TestCase("integer        ", 12312, 12312)]
        [TestCase("exponent       ", 1e-6, 1e-6)]
        [TestCase("long           ", 9999999999999L, 9999999999999L)]
        [TestCase("bool true      ", true, true)]
        [TestCase("bool false     ", false, false)]
        [TestCase("string true    ", "true", "true")]
        [TestCase("string false   ", "false", "false")]
        [TestCase("BMP chars      ", "éáéú€ΑΒΓΔΕΖໝ‱ㄓ龜龜契金喇网ꬤ￦", "éáéú€ΑΒΓΔΕΖໝ‱ㄓ龜龜契金喇网ꬤ￦")]
        [TestCase("SMP chars      ", "🌍🍈👽💔", "🌍🍈👽💔")]
        [TestCase("SIP chars      ", "𠀀𠀁𠀂𠀃𤁴𤁵𤁶𫜲𫜳𫜴", "𠀀𠀁𠀂𠀃𤁴𤁵𤁶𫜲𫜳𫜴")]
        [TestCase("SURROGATE char ", "𠈓", "𠈓")]
        [TestCase("NULL ending    ", "hhhhghg\0", "hhhhghg\0")]
        [TestCase("empty string   ", "", "")]
        [TestCase("null value     ", null, null)]
        public void TC_ShouldSerializePrimitive(string info, object input, object expected) {
            var output = SerializeAndDeserialize(input);
            A.AreEqual(expected, output, info);
        }

        [Test]
        public void TC_ShouldDeserializePNGBase64Image() {
            using (var bitmap = new System.Drawing.Bitmap(1024 * 2, 768 * 2)) {
                bitmap.SetPixel(1, 1, System.Drawing.Color.Bisque);
                bitmap.SetPixel(2, 2, System.Drawing.Color.Honeydew);
                bitmap.SetPixel(3, 3, System.Drawing.Color.PapayaWhip);
                Image input = new Image(bitmap);

                var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                byte[] expected = ms.ToArray();
                string str = Convert.ToBase64String(expected);
                Image output = (Image)SerializeAndDeserialize(str);

                A.AreEqual(input.CRC, output.CRC);
            }
        }

        [Test]
        public void TC_ShouldSerializeList() {
            var input = new List { "a", 7, 8 };
            var output = (List)SerializeAndDeserialize(input);
            CA.AreEqual(input, output);
        }

        [Test]
        public void TC_ShouldSerializeDictionary() {
            var input = new Dictionary{
                {"a", "abcdefg \" ' 0123 \0 \u0234 "},
                {"b", 12312},
                {"c", true},
                {"d", false},
                {"e", null}
            };
            var output = (Dictionary)SerializeAndDeserialize(input);
            CA.AreEqual(input, output);
        }

        [Test]
        public void TC_ShouldSerializeZipFile() {
            string testfolder = Path.Combine(System.IO.Path.GetTempPath(), DateTime.Now.Ticks.ToString());
            Directory.CreateDirectory(testfolder);

            string filepath = Path.Combine(testfolder, "text1.txt");
            string expectedText = "START data\r\n ééúóáEN\u0247D";
            using (StreamWriter sw = File.CreateText(filepath))
                sw.Write(expectedText);

            try {
                string output;
                using (var zip = new ZipFile()) {
                    zip.AddFile(filepath);
                    File.Delete(filepath);
                    output = (string)SerializeAndDeserialize(zip);
                }
                byte[] bytes = Convert.FromBase64String(output);


                //extract the text file
                using (var stream = new MemoryStream(bytes))
                    ZipFile.ExtractAll(stream, testfolder);

                //assert text content
                var textReaded = File.ReadAllText(filepath);
                A.AreEqual(expectedText, textReaded);
            } finally {
                Directory.Delete(testfolder, true);
            }
        }
        
        [Test]
        public void TC_ShouldDeserializePng_Pad0() {
            string base64 =
@"""iVBORw0KGgoAAAANSUhEUgAAAAEAAAACCAMAAACuX0YVAAAABlBMVEUAAAD/
    //+l2Z/dAAAADElEQVQI12NgZGAAAAAHAAI4McYTAAAAAElFTkSuQmCC   """;
            byte[] base64Bytes = System.Text.Encoding.ASCII.GetBytes(base64);
            var output = (Image)JSON.Parse(base64Bytes, base64Bytes.Length);
            A.AreEqual(1, output.Width);
            A.AreEqual(2, output.Height);
            A.AreEqual("2FC51328", output.CRC);
            output.Dispose();
        }
        
        [Test]
        public void TC_ShouldDeserializePng_Pad1() {
            string base64 =
@"""iVBORw0KGgoAAAANSUhEUgAAAAMAAAACCAMAAACqqpYoAAAABlBMVEUAAAD/
    //+l2Z/dAAAADklEQVR4AWNgZGRkABIAAB0ABroxs5IAAAAASUVORK5CYII=""";
            byte[] base64Bytes = System.Text.Encoding.ASCII.GetBytes(base64);
            var output = (Image)JSON.Parse(base64Bytes, base64Bytes.Length);
            A.AreEqual(3, output.Width);
            A.AreEqual(2, output.Height);
            A.AreEqual("909C5733", output.CRC);
            output.Dispose();
        }

        [Test]
        public void TC_ShouldDeserializePng_Pad2() {
            string base64 =
@"""iVBORw0KGgoAAAANSUhEUgAAAAMAAAAECAMAAAB883U1AAAACVBMVEUAAAD/
    AAD///9nGWQeAAAAE0lEQVR4AWNgYmICYkYGBjBmAgAArgATGVgZTQAAAABJ
    RU5ErkJggg==                                                """;
            byte[] base64Bytes = System.Text.Encoding.ASCII.GetBytes(base64);
            var output = (Image)JSON.Parse(base64Bytes, base64Bytes.Length);
            A.AreEqual(3, output.Width);
            A.AreEqual(4, output.Height);
            A.AreEqual("7286E2FE", output.CRC);
            output.Dispose();
        }

        [Test]
        public void TC_ShouldSerializeDeepStructure() {
            var input = new Dictionary{
                {"a", "abcdefg \" ' 0123 \0 \u0234 "},
                {"b", 12312},
                {"d", 's'},
                {"e", new Dictionary{
                    {"aa", "hsgfd"},
                    {"bb", 276534},
                    {"cc", 2222.99},
                    {"dd", 'u'},
                    {"ee", new object[]{23, 989, "lkjlj"}}
                }},
                {"c", 1e-6},
            };

            var r = (Dictionary)SerializeAndDeserialize(input);

            A.AreEqual(r.Count, 5);
            A.AreEqual(r["a"], "abcdefg \" ' 0123 \0 \u0234 ");
            A.AreEqual(r["b"], 12312);
            A.AreEqual(r["c"], 1e-6);
            A.AreEqual(r["d"], "s");

            var e = (Dictionary)r["e"];
            A.AreEqual(e.Count, 5);
            A.AreEqual(e["aa"], "hsgfd");
            A.AreEqual(e["bb"], 276534);
            A.AreEqual(e["cc"], 2222.99);
            A.AreEqual(e["dd"], "u");

            var ee = (List)e["ee"];
            A.AreEqual(ee.Count, 3);
            A.AreEqual(ee[0], 23);
            A.AreEqual(ee[2], "lkjlj");
        }

        private static object SerializeAndDeserialize(object input) {
            var writer = JSON.Serialize(input);
            object result = JSON.Parse(writer.GetBuffer(), (int)writer.Length);
            return result;
        }

    }

}
