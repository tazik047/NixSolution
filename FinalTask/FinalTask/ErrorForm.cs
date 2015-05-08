using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalTask
{
    public partial class ErrorForm : Form
    {

        public ErrorForm()
        {
            InitializeComponent();
        }

        public bool HaveError
        {
            get { return !String.IsNullOrEmpty(richTextBox1.Text); }
        }

        public void ClearError()
        {
            richTextBox1.Text = "";
        }

        public void AddError(String message)
        {
            if (HaveError)
            {
                richTextBox1.AppendText("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                richTextBox1.AppendText(Environment.NewLine);
            }
            richTextBox1.AppendText(message);
            richTextBox1.AppendText(Environment.NewLine);
        }

        private void ErrorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
