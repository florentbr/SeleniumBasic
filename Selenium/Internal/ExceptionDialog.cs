using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Selenium.Internal {

    partial class ExceptionDialog : Form {

        private StringBuilder _sb;

        public ExceptionDialog(Exception exception) {
            System.Windows.Forms.Application.EnableVisualStyles();
            InitializeComponent();

            var assembly = typeof(ExceptionDialog).Assembly;
            var att = assembly.GetFirstAttribute<AssemblyURLAttribute>();
            this.linkLabel1.Text = att.URL;

            _sb = new StringBuilder();
            while (exception != null) {
                _sb.AppendFormat("{0}: {1}\r\n", exception.GetType(), exception.Message);
                _sb.AppendLine(exception.StackTrace);
                _sb.AppendLine();
                exception = exception.InnerException;
            }
            textBox1.Text = _sb.ToString();
            textBox1.SelectionStart = 0;
            textBox1.SelectionLength = 0;
            textBox1.KeyDown += textBox1_KeyDown;
        }

        private void btCopy_Click(object sender, EventArgs e) {
            ClipboardExt.SetText(_sb.ToString());
        }

        private void btClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == System.Windows.Forms.Keys.A && e.Control) {
                textBox1.SelectAll();
                e.Handled = true;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(((LinkLabel)sender).Text);
        }

        internal static void ShowDialog(Exception exception) {
            new ExceptionDialog(exception).ShowDialog();
        }
    }

}
