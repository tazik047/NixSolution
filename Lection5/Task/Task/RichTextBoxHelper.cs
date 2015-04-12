using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task
{
    //Вспомогательный класс, содержащий методы расширения для RichTextBox.
    static class RichTextBoxHelper
    {
        public static void AppendBoldTextWithColor(this RichTextBox richTextBox, string text, Color color)
        {
            Color defaultColor = richTextBox.SelectionColor;
            richTextBox.SelectionColor = color;
            AppendBoldText(richTextBox, text);
            richTextBox.SelectionColor = defaultColor;

        }

        public static void AppendBoldText(this RichTextBox richTextBox, string text)
        {
            int length = richTextBox.TextLength;
            Font defaultFont = richTextBox.SelectionFont;
            richTextBox.SelectionFont = new Font(defaultFont, FontStyle.Bold);
            richTextBox.AppendText(text);
            richTextBox.SelectionFont = defaultFont;
        }
    }
}
