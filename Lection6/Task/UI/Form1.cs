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
        private bool startEditOrAdd;

        private List<Control> inputs;

        private Contact selectedContact
        {
            get
            {
                if (groupCheckBox.Checked)
                {
                    return (treeView1.SelectedNode == null || treeView1.SelectedNode.Name == "") ?
                        null : manager.GetContactById(treeView1.SelectedNode.Name);
                }
                return listBox1.SelectedItem == null ? null : (Contact)listBox1.SelectedItem;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                manager = new ContactManager("data.xml");
                fillList(manager.Contacts);
                inputs = new List<Control>()
                {
                    SurnameTextBox,
                    NameTextBox,
                    GroupTextBox,
                    PhoneMaskedTextBox,
                    MobliePhoneMaskedTextBox
                };
            }
            catch (System.Xml.XmlException ex)
            {
                MessageBox.Show("Не удалось открыть базу контактов.\n" +
                    "Замените файл data.xml предыдущим рабочим файлом или удалите его.\n" +
                    "Дополнительная информация: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (!checkBeforeStartMethod())
                return;
            resetInputs(true);
            startEditOrAdd = true;
            hideOrShowControlsForAdd(true);
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            var c = selectedContact;
            if (c == null)
                return;
            enableOrDisableControlsForEdit(true);
            startEditOrAdd = true;

        }

        private void delButton_Click(object sender, EventArgs e)
        {
            var c = selectedContact;
            if (c == null || !checkBeforeStartMethod())
                return;
            if (MessageBox.Show("Вы уверены, что хотите удалить " + c.ToString(),
                "Предупреждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information) == DialogResult.No)
                return;
            manager.Remove(c.Id);
            if (groupCheckBox.Checked)
                treeView1.SelectedNode.Remove();
            else
                listBox1.Items.Remove(c);
        }

        private void resetInputs(bool clearText = false)
        {
            foreach (var control in inputs)
            {
                control.BackColor = Color.White;
                if (clearText) control.Text = "";
            }
            pictureBox1.BackgroundImage = Properties.Resources.unk;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            bool hasError = false;
            foreach (var control in inputs)
            {
                if (string.IsNullOrWhiteSpace(control.Text))
                {
                    hasError = true;
                    control.BackColor = Color.Red;
                }
            }
            if (hasError)
            {
                MessageBox.Show("Не все поля заполнены", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            resetInputs();
            if (adding)
            {
                var c = new Contact()
                {
                    Surname = SurnameTextBox.Text,
                    Name = NameTextBox.Text,
                    Group = GroupTextBox.Text,
                    Phone = PhoneMaskedTextBox.Text,
                    MobilePhone = MobliePhoneMaskedTextBox.Text,
                    Photo = new Bitmap(pictureBox1.BackgroundImage)
                };
                manager.Add(c);
                listBox1.Items.Add(c);
                hideOrShowControlsForAdd(false);
            }
            else if (editing)
            {
                selectedContact.Surname = SurnameTextBox.Text;
                selectedContact.Name = NameTextBox.Text;
                selectedContact.Group = GroupTextBox.Text;
                selectedContact.Phone = PhoneMaskedTextBox.Text;
                selectedContact.MobilePhone = MobliePhoneMaskedTextBox.Text;
                selectedContact.Photo = new Bitmap(pictureBox1.BackgroundImage);
                manager.Update(selectedContact);
                enableOrDisableControlsForEdit(false);
            }
            startEditOrAdd = false;
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!groupCheckBox.Checked)
                fillList(manager.SearchContactList(searchTextBox.Text));
            else
                fillTree(manager.SearchContactDictionary(searchTextBox.Text));
        }

        private void GroupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            listBox1.Visible = !groupCheckBox.Checked;
            treeView1.Visible = groupCheckBox.Checked;
            searchTextBox.Text = "";
            if (groupCheckBox.Checked)
                fillTree(manager.GroupContact);
            else
                fillList(manager.Contacts);
        }

        private void fillTree(Dictionary<string, List<Contact>> dict)
        {
            treeView1.Nodes.Clear();
            foreach (var group in dict)
            {
                var node = new TreeNode(group.Key);
                foreach (var contact in group.Value)
                    node.Nodes.Add(contact.Id, contact.ToString());
                treeView1.Nodes.Add(node);
            }
        }

        private void fillList(List<Contact> list)
        {
            listBox1.DataSource = list;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!checkBeforeStartMethod()) return;
            var c = selectedContact;
            if (c == null)
            {
                hideOrShowControlsForAdd(false);
                resetInputs(true);
                return;
            }
            SurnameTextBox.Text = c.Surname;
            NameTextBox.Text = c.Name;
            GroupTextBox.Text = c.Group;
            PhoneMaskedTextBox.Text = c.Phone;
            MobliePhoneMaskedTextBox.Text = c.MobilePhone;
            pictureBox1.BackgroundImage = c.Photo;
            editing = false;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listBox1_SelectedIndexChanged(sender, e);
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
            inputs.ForEach(c => c.Enabled = enable);
            pictureBox1.Enabled = enable;

            saveButton.Enabled = enable;
            loadButton.Visible = enable;
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
                    resetInputs(true);
                    editing = false;
                    return true;
                }
                return false;
            }
            if (startEditOrAdd)
            {
                hideOrShowControlsForAdd(false);
                resetInputs(true);
                editing = false;
                startEditOrAdd = false;
            }
            return true;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            editing = true;
        }
    }
}
