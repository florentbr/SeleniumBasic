using System.Collections.Generic;
using System.IO;

namespace Selenium.Pdf {

    class PdfOutlines {

        class PdfOutline {
            public int Id;
            public string Text;
            public int Page;
            public int Y;
        }

        private List<PdfOutline> _outlines = new List<PdfOutline>();

        public int Count {
            get {
                return _outlines.Count;
            }
        }

        public void Add(int id, string text, int page, int y) {
            _outlines.Add(new PdfOutline {
                Id = id,
                Text = text,
                Page = page,
                Y = y
            });
        }

        public void Write(StreamWriter writer, PdfXRefs xrefs) {

            //Write the outlines entries
            int count = _outlines.Count;
            for (int i = 0; i < count; i++) {
                PdfOutline outline = _outlines[i];
                xrefs.RegisterObject(outline.Id);
                writer.WriteLine(outline.Id + " 0 obj");
                writer.WriteLine("<<");
                writer.WriteLine("/Title (" + PdfWriter.EscapeText(outline.Text) + ")");
                writer.WriteLine("/Parent " + PdfXRefs.ID_OUTLINES + " 0 R");
                writer.Write("/Dest [ ");
                writer.Write(outline.Page);
                writer.Write(" 0 R /XYZ null ");
                writer.Write(outline.Y == -1 ? "null" : outline.Y.ToString());
                writer.WriteLine(" 0 ]");
                if (i > 0)
                    writer.WriteLine("/Prev " + _outlines[i - 1].Id + " 0 R");
                if (i < count - 1)
                    writer.WriteLine("/Next " + _outlines[i + 1].Id + " 0 R");
                writer.WriteLine(">>");
                writer.WriteLine("endobj");
            }

            //Write the main entry
            xrefs.RegisterObject(PdfXRefs.ID_OUTLINES);
            writer.WriteLine(PdfXRefs.ID_OUTLINES + " 0 obj");
            writer.WriteLine("<<");
            writer.WriteLine("/Type /Outlines");
            if (_outlines.Count == 0) {
                writer.WriteLine("/Count 0");
                writer.WriteLine("/First null");
                writer.WriteLine("/Last null");
            } else {
                writer.WriteLine("/Count " + count);
                writer.WriteLine("/First " + _outlines[0].Id + " 0 R");
                writer.WriteLine("/Last " + _outlines[count - 1].Id + " 0 R");
            }
            writer.WriteLine(">>");
            writer.WriteLine("endobj");
        }

    }

}
