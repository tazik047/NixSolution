using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core;

namespace UI
{
    public partial class Form1 : Form
    {
        private ContactManager manager;
        private bool adding;
        private bool editing;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            manager = new ContactManager("data.xml");
            var c = manager.Contacts;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (!checkBeforeStartMethod())
                return;
            hideOrShowControlsForAdd(true);
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            enableOrDisableControlsForEdit(true);
        }

        private void delButton_Click(object sender, EventArgs e)
        {

        }

        private void saveButton_Click(object sender, EventArgs e)
        {

        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void GroupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            listBox1.Visible = !groupCheckBox.Checked;
            groupBox1.Visible = groupCheckBox.Checked;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }


        private void hideOrShowControlsForAdd(bool show)
        {
            label1.Visible = show;
            label2.Visible = show;
            label3.Visible = show;
            label4.Visible = show;
            label5.Visible = show;
            loadButton.Visible = show;
            editButton.Visible = !show;
            delButton.Visible = !show;
            adding = show;
            enableOrDisableControlsForEdit(show);
        }

        private void enableOrDisableControlsForEdit(bool enable)
        {
            SurnameTextBox.Enabled = enable;
            NameTextBox.Enabled = enable;
            GroupTextBox.Enabled = enable;
            PhoneMaskedTextBox.Enabled = enable;
            MobliePhoneMaskedTextBox.Enabled = enable;
            saveButton.Enabled = enable;
            loadButton.Enabled = enable;
            editButton.Enabled = !enable;
        }

        private bool checkBeforeStartMethod()
        {
            if (adding && editing)
            {
                if (MessageBox.Show("Новый контакт еще не сохранен.\nПри продолжение новые данные будут утеряны",
                    "Вы уверены?", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    hideOrShowControlsForAdd(false);
                    return true;
                }
                return false;
            }
            if (editing)
            {
                if (MessageBox.Show("Контакт не сохранен.\nПри продолжение новые данные будут утеряны",
                    "Вы уверены?", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    enableOrDisableControlsForEdit(false);
                    editing = false;
                    return true;
                }
                return false;
            }
            return true;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            editing = true;
        }
    }
}
