using System;
using System.Windows.Forms;

namespace FinalTask
{
    /// <summary>
    /// Форма для отображения ошибок.
    /// </summary>
    public partial class ErrorForm : Form
    {

        public ErrorForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Показывает содержит ли форма ошибки.
        /// </summary>
        public bool HaveError
        {
            get { return !String.IsNullOrEmpty(richTextBox1.Text); }
        }

        /// <summary>
        /// Очищает форму от всех ошибок.
        /// </summary>
        public void ClearError()
        {
            richTextBox1.Text = "";
        }

        /// <summary>
        /// Длбавляет на форму новую ошибку.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public void AddError(String message)
        {
            if (HaveError) // Если есть ошибки то необходимо добавить разделитель между ними.
            {
                richTextBox1.AppendText("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                richTextBox1.AppendText(Environment.NewLine);
            }
            richTextBox1.AppendText(message);
            richTextBox1.AppendText(Environment.NewLine);
        }

        /// <summary>
        /// Просто скрываем форму при закрытии для того что бы форма существовала все время.
        /// </summary>
        private void ErrorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
