using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core;

namespace UI
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Предоставляет средства для управления всеми контактами.
        /// </summary>
        private ContactManager _manager;

        /// <summary>
        /// true - если открыт только режим добавления нового контакта.
        /// false - если добавление нового пользователя не активно.
        /// </summary>
        private bool _adding;
        
        /// <summary>
        /// true - если были внесены какие-либо изменения при редактировании или добавлении контакта.
        /// false - если не было изменений контакта. 
        /// </summary>
        private bool _editing;

        /// <summary>
        /// true - если открыт режим добавления или редактирования контакта.
        /// false - если контакт не редактируется и не добавляется новый.
        /// </summary>
        private bool _startEditOrAdd;

        /// <summary>
        ///  Выбранный элемент для редактирования.
        /// </summary>
        private Contact _selectedContactForEditing;

        /// <summary>
        /// Список текстовых полей, в которые будут записывать значения полей контакта.
        /// </summary>
        private List<Control> _inputs;

        /// <summary>
        /// Свойство, возвращающее выбранный контакт из списка или дерева.
        /// Если контакт не выбран то возращает null.
        /// </summary>
        private Contact _selectedContact
        {
            get
            {
                if (groupCheckBox.Checked)
                {
                    return (treeView1.SelectedNode == null || treeView1.SelectedNode.Name == "") ?
                        null : _manager.GetContactById(treeView1.SelectedNode.Name);
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
                _manager = new ContactManager("data.xml");
                fillList(_manager.Contacts);
                _inputs = new List<Control>()
                {
                    surnameTextBox,
                    nameTextBox,
                    groupTextBox,
                    phoneMaskedTextBox,
                    mobliePhoneMaskedTextBox
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

        /// <summary>
        /// Обработчик события при нажатии на кнопку добавления нового контакта.
        /// </summary>
        private void addButton_Click(object sender, EventArgs e)
        {
            if (!checkBeforeStartMethod())
                return;
            _selectedContactForEditing = _selectedContact;
            resetInputs(true);
            _startEditOrAdd = true;
            hideOrShowControlsForAdd(true);
        }

        /// <summary>
        /// Обработчик события при нажатии на кнопку редактировании контакта.
        /// </summary>
        private void editButton_Click(object sender, EventArgs e)
        {
            if (!checkContactIsSelected()) return;
            _selectedContactForEditing = _selectedContact;
            enableOrDisableControlsForEdit(true);
            _startEditOrAdd = true;

        }

        /// <summary>
        /// Проверяет выбран ли контакт. Если контакт не выбран, то создается сообщение для пользователя.
        /// </summary>
        /// <returns>true - контакт выбран
        /// false - контакт не выбран</returns>
        private bool checkContactIsSelected()
        {
            if (_selectedContact == null)
            {
                MessageBox.Show("Сначала нужно выбрать контакт.",
                    "Предупреждение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Обработчик события при нажатии на кнопку удаления контакта.
        /// </summary>
        private void delButton_Click(object sender, EventArgs e)
        {
            if (!checkContactIsSelected()) return;
            var c = _selectedContact;
            if (MessageBox.Show("Вы уверены, что хотите удалить " + c.ToString(),
                "Предупреждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information) == DialogResult.No)
                return;
            _manager.Remove(c.Id);
            if (groupCheckBox.Checked)
                treeView1.SelectedNode.Remove();
            else
                listBox1.DataSource = _manager.Contacts;
            if (_startEditOrAdd)
            {
                hideOrShowControlsForAdd(false);
                _editing = false;
            }
        }

        /// <summary>
        /// Сбрасывает состояние полей формы в начальное.
        /// </summary>
        /// <param name="clear">false - сбрасывает только цвет полей в белый
        /// true - сбрасывает цвет и очищает текст этих полей и выставлет изображение по умолчанию.</param>
        private void resetInputs(bool clear = false)
        {
            foreach (var control in _inputs)
            {
                control.BackColor = Color.White;
                if (clear) control.Text = "";
            }
            if (clear)
            {
                _editing = false;
                pictureBox1.BackgroundImage = Properties.Resources.unk;
            }
        }

        /// <summary>
        /// Обработчик события при нажатии на кнопку сохранения контакта.
        /// </summary>
        private void saveButton_Click(object sender, EventArgs e)
        {
            bool hasError = false; //Флаг наличия ошибок на форме.
            resetInputs();
            //Проверяем поля на пустоту, если поле пустое, то помечаем его красным цветом
            foreach (var control in _inputs.Where(control => string.IsNullOrWhiteSpace(control.Text)))
            {
                hasError = true;
                control.BackColor = Color.Red;
            }
            // Поля MaskedTextBox должны быть заполнены польностью
            foreach (var control in _inputs.OfType<MaskedTextBox>().Where(t => t.Text.Length != t.Mask.Length))
            {
                hasError = true;
                control.BackColor = Color.Red;
            }

            if (hasError)
            {
                MessageBox.Show("Не все поля заполнены", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_adding)
            {
                var c = new Contact()
                {
                    Surname = surnameTextBox.Text,
                    Name = nameTextBox.Text,
                    Group = groupTextBox.Text,
                    Phone = phoneMaskedTextBox.Text,
                    MobilePhone = mobliePhoneMaskedTextBox.Text,
                    Photo = new Bitmap(pictureBox1.BackgroundImage)
                };
                _manager.Add(c);

            }
            else if (_editing)
            {
                _selectedContactForEditing.Surname = surnameTextBox.Text;
                _selectedContactForEditing.Name = nameTextBox.Text;
                _selectedContactForEditing.Group = groupTextBox.Text;
                _selectedContactForEditing.Phone = phoneMaskedTextBox.Text;
                _selectedContactForEditing.MobilePhone = mobliePhoneMaskedTextBox.Text;
                _selectedContactForEditing.Photo = new Bitmap(pictureBox1.BackgroundImage);
                _manager.Update(_selectedContactForEditing);
            }
            _startEditOrAdd = false;
            hideOrShowControlsForAdd(false);
            _editing = false;
            if (groupCheckBox.Checked)
                fillTree(_manager.GroupContact);
            else
                fillList(_manager.Contacts);
            searchTextBox.Text = "";
        }

        /// <summary>
        /// Обработчик события при вводе текста в поле для поиска контактов.
        /// </summary>
        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (groupCheckBox.Checked)
                fillTree(_manager.SearchContactDictionary(searchTextBox.Text));
            else
                fillList(_manager.SearchContactList(searchTextBox.Text));
        }

        /// <summary>
        /// Обработчик события переключения вида списка контактов между списком и деревом.
        /// </summary>
        private void GroupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            listBox1.Visible = !groupCheckBox.Checked;
            treeView1.Visible = groupCheckBox.Checked;
            searchTextBox.Text = "";
            if (groupCheckBox.Checked)
                fillTree(_manager.GroupContact);
            else
                fillList(_manager.Contacts);
            listBox1_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// Заполняет дерево контактов в соответствии с задаными значениями.
        /// </summary>
        /// <param name="dict">Содержит контакты сгруппированные по группе</param>
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

        /// <summary>
        /// Заполняет список контактов заданными значениями
        /// </summary>
        /// <param name="list">Список контактов для заполнения</param>
        private void fillList(List<Contact> list)
        {
            listBox1.DataSource = list;
        }

        /// <summary>
        /// Обработчик события выбора контакта.
        /// </summary>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!checkBeforeStartMethod()) return;
            var c = _selectedContact;
            if (c == null)
            {
                resetInputs(true);
                return;
            }
            surnameTextBox.Text = c.Surname;
            nameTextBox.Text = c.Name;
            groupTextBox.Text = c.Group;
            phoneMaskedTextBox.Text = c.Phone;
            mobliePhoneMaskedTextBox.Text = c.MobilePhone;
            pictureBox1.BackgroundImage = c.Photo;
            _editing = false;
        }

        /// <summary>
        /// Обработчик события выбора контакта из дерева.
        /// </summary>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listBox1_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// Вспомогательный метод скрывающий или показывающий надписи или кнопки для создания нового контакта.
        /// </summary>
        /// <param name="show">true - показать элементы
        /// false - скрыть элементы</param>
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
            _adding = show;
            enableOrDisableControlsForEdit(show);
        }

        /// <summary>
        /// Включает или выключает поля и кнопки, необходимые для редактирования полей контакта.
        /// </summary>
        /// <param name="enable">true - включить поля и кнопки
        /// false - выключить поля и кнопки</param>
        private void enableOrDisableControlsForEdit(bool enable)
        {
            _inputs.ForEach(c => c.Enabled = enable);
            pictureBox1.Enabled = enable;
            saveButton.Enabled = enable;
            loadButton.Visible = enable;
            editButton.Enabled = !enable;
        }

        /// <summary>
        /// Вспомогательный метод, проверяющий проходит ли редактирования или создания контакта, 
        /// для предотвращения случайной потери внесенных изменений.
        /// </summary>
        /// <returns>true - можно выполнить следущий метод
        /// false - нельзя выполнять следущий метод</returns>
        private bool checkBeforeStartMethod()
        {
            if (_adding && _editing) // создается новый контакт и внесина какая-либо информация
            {
                if (MessageBox.Show("Новый контакт еще не сохранен.\nПри продолжение новые данные будут утеряны",
                    "Вы уверены?", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    hideOrShowControlsForAdd(false);
                    _editing = false;
                    return true;
                }
                return false;
            }
            if (_editing) //какая-либо информация изменена
            {
                if (MessageBox.Show("Контакт не сохранен.\nПри продолжение новые данные будут утеряны",
                    "Вы уверены?", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    enableOrDisableControlsForEdit(false);
                    resetInputs(true);
                    _editing = false;
                    return true;
                }
                return false;
            }
            if (_startEditOrAdd) //форма редактирования или создания открыта, но нет никаких изменений, поэтому можно ее закрыть
            {
                hideOrShowControlsForAdd(false);
                resetInputs(true);
                _editing = false;
                _startEditOrAdd = false;
            }
            return true;
        }

        /// <summary>
        /// Обработчик изменеия полей формы.
        /// </summary>
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            _editing = true;
        }

        /// <summary>
        /// Загружает новое изображение для контакта.
        /// </summary>
        private void loadButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.BackgroundImage = new Bitmap(openFileDialog1.FileName);
                _editing = true; // значение полей изменено.
            }
        }

        /// <summary>
        /// Обработчик закрытия формы для проверки того, сохранил ли пользователь свои данные.
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !checkBeforeStartMethod();
        }
    }
}
