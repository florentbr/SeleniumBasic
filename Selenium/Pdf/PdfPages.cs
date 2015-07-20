using System.Collections.Generic;
using System.IO;

namespace Selenium.Pdf {

    class PdfPages {

        private List<PdfPage> _pages = new List<PdfPage>();

        public PdfPage this[int index]{
            get {
                return _pages[index];
            }
        }

        public int Count {
            get {
                return _pages.Count;
            }
        }

        public PdfPage Add(int id, int widthPt, int heightPt) {
            PdfPage page = new PdfPage(id, widthPt, heightPt);
            _pages.Add(page);
            return page;
        }

        /// <summary>
        /// Writes the pages
        /// </summary>
        public void Write(StreamWriter writer, PdfXRefs xrefs) {
            int count = _pages.Count;
            foreach (PdfPage page in _pages) {
                xrefs.RegisterObject(page.Id);
                writer.WriteLine(page.Id + " 0 obj");
                writer.WriteLine("<<");
                writer.WriteLine("/Type /Page");
                writer.WriteLine("/Parent " + PdfXRefs.ID_PAGES + " 0 R");
                writer.WriteLine("/MediaBox [0 0 " + page.Width + " " + page.Height + "]");
                writer.Write("/Contents [ ");
                foreach (int id in page.Contents)
                    writer.Write(id + " 0 R ");
                writer.WriteLine(']');
                writer.WriteLine("/Resources <<");
                writer.WriteLine(" /ProcSet [/PDF/Text/ImageC]");
                writer.WriteLine(" /Font " + PdfXRefs.ID_FONTS + " 0 R");
                if (page.XObjects.Count > 0) {
                    writer.Write(" /XObject << ");
                    foreach (var item in page.XObjects)
                        writer.Write(item.CodeName + " " + item.Id + " 0 R ");
                    writer.WriteLine(">>");
                }
                writer.WriteLine(">>");
                if (page.Annots.Count > 0) {
                    writer.Write("/Annots[ ");
                    foreach (int id in page.Annots) {
                        writer.Write(id + " 0 R ");
                    }
                    writer.WriteLine(']');
                }
                writer.WriteLine(">>");
                writer.WriteLine("endobj");
            }

            //Writes the pages objects
            xrefs.RegisterObject(PdfXRefs.ID_PAGES);

            writer.WriteLine(PdfXRefs.ID_PAGES + " 0 obj");
            writer.WriteLine("<<");
            writer.WriteLine("/Type /Pages");
            writer.Write("/Kids [ ");
            foreach (PdfPage page in _pages) {
                writer.Write(page.Id + " 0 R ");
            }
            writer.WriteLine("]");
            writer.WriteLine("/Count " + _pages.Count);
            writer.WriteLine(">>");
            writer.WriteLine("endobj");
        }

    }
}
