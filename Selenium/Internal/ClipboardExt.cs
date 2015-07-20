using System.Drawing;
using System.Windows.Forms;

namespace Selenium.Internal {

    class ClipboardExt {

        public static void SetText(string value) {
            ThreadExt.RunSTA(() => {
                Clipboard.SetText(value, TextDataFormat.UnicodeText);
                return 0;
            }, 6000);
        }

        public static void SetImage(Bitmap value) {
            ThreadExt.RunSTA(() => {
                Clipboard.SetImage(value);
                return 0;
            }, 6000);
        }

        public static string GetText() {
            string result = ThreadExt.RunSTA(() => {
                string text = Clipboard.GetText();
                return text;
            }, 6000);
            return result;
        }

    }

}
