using System.Collections.Generic;
using System.IO;

namespace Selenium.Pdf {

    class PdfPage {

        public class XObject {
            public int Id;
            public string CodeName;
        }

        public readonly int Id;
        public int Height;
        public int Width;
        public readonly List<int> Contents = new List<int>();
        public readonly List<int> Annots = new List<int>();
        public readonly List<XObject> XObjects = new List<XObject>();

        public PdfPage(int id, int width, int height) {
            Id = id;
            Height = height;
            Width = width;
        }

        internal void AddContent(int id) {
            Contents.Add(id);
        }

        internal void AddXObject(int id, string codeName) {
            XObjects.Add(new XObject {
                Id = id,
                CodeName = codeName
            });
        }

        internal void AddAnnot(int id) {
            Annots.Add(id);
        }

    }

}
