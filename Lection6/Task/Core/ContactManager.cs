using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Core
{
    /// <summary>
    /// Класс для управления списком контактов.
    /// </summary>
    public class ContactManager
    {
        /// <summary>
        /// XML Документ.
        /// </summary>
        private readonly XDocument _contacts;

        /// <summary>
        /// Путь к файлу, в котором хранятся данные.
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// Список сохраненных контактов.
        /// </summary>
        public List<Contact> Contacts
        {
            get { return _contacts.Root.Elements().Select(getContact).ToList(); }
        }

        /// <summary>
        /// Контакты сгруппированные по группе.
        /// </summary>
        public Dictionary<string, List<Contact> > GroupContact 
        {
            get
            {
                return Contacts.GroupBy(c => c.Group)
                    .Select(gr => gr.ToList())
                    .ToDictionary(gr => gr[0].Group);
            }
        }

        public ContactManager(string path)
        {
            _path = path;
            if (!File.Exists(path)) // если программа запускается первый раз
            {
                _contacts = new XDocument(new XElement("Contacts"));
                _contacts.Save(path);
            }
            else
                _contacts = XDocument.Load(path);
        }

        /// <summary>
        /// Выполняет поиск контактов содержащих в имени или фамилии заданный текст и возвращает список.
        /// </summary>
        /// <param name="name">Текст для поиска</param>
        /// <returns>Подходящий список контактов</returns>
        public List<Contact> SearchContactList(string name)
        {
            var names = name.Split().Where(s=>!string.IsNullOrWhiteSpace(s)).Select(s=>s.ToUpper());
            if (!names.Any()) return Contacts;

            return _contacts.Root.Elements()
                .Where(e => names.All(
                    name1 => e.Element("Name").Value.ToUpper().Contains(name1) 
                        || e.Element("Surname").Value.ToUpper().Contains(name1)
                    ))
                .Select(getContact).ToList();
        }

        /// <summary>
        /// Выполняет поиск контактов содержащих в имени или фамилии заданный текст 
        /// и возвращает контакты сгруппированные по группе.
        /// </summary>
        /// <param name="name">Текст для поиска</param>
        /// <returns>Подходящий контакты сгруппированные по группе.</returns>
        public Dictionary<string, List<Contact>> SearchContactDictionary(string name)
        {
            return SearchContactList(name).GroupBy(c => c.Group)
                .Select(gr => gr.ToList())
                .ToDictionary(gr => gr[0].Group);
        } 

        /// <summary>
        /// Выполняет поиск контакта по заданному Id/
        /// </summary>
        /// <param name="id">Id контакта</param>
        /// <returns>Найденный контакт</returns>
        public Contact GetContactById(string id)
        {
            return getContact(_contacts.Root
                                    .Elements().First(e => e.Element("Id")
                                    .Value.Equals(id)));
        }

        /// <summary>
        /// Добавляет новый контакт.
        /// </summary>
        /// <param name="contact">Новый контакт</param>
        public void Add(Contact contact)
        {
            _contacts.Root.Add(getXElement(contact));
            _contacts.Save(_path);
        }

        /// <summary>
        /// Удаляет выбранный контакт.
        /// </summary>
        /// <param name="id">Id контакта для удаления</param>
        public void Remove(string id)
        {
            _contacts.Root.Elements().First(e=>e.Element("Id").Value.Equals(id)).Remove();
            _contacts.Save(_path);
        }

        /// <summary>
        /// Обновляет контакт с таким же id как и у ранее сохраненного.
        /// </summary>
        /// <param name="contact">Контакт для обновления</param>
        public void Update(Contact contact)
        {
            var current = _contacts.Root.Elements().First(e => e.Element("Id").Value.Equals(contact.Id));
            current.ReplaceWith(getXElement(contact));
            _contacts.Save(_path);
        }

        /// <summary>
        /// Преобразовует контакт в элемент XML.
        /// </summary>
        /// <param name="contact">Контакт для преобразования</param>
        /// <returns>Результирующий XML элемент</returns>
        private XElement getXElement(Contact contact)
        {
            return new XElement("Contact",
                new XElement("Id", contact.Id),
                new XElement("Surname", contact.Surname),
                new XElement("Name", contact.Name),
                new XElement("Group", contact.Group),
                new XElement("Phone", contact.Phone),
                new XElement("MobilePhone", contact.MobilePhone),
                new XElement("Photo", convertBitmapToString(contact.Photo)));
        }

        /// <summary>
        /// Преобразует XML элемент в контакт.
        /// </summary>
        /// <param name="element">XML элемент для преобразования</param>
        /// <returns>Результирующий контакт</returns>
        private Contact getContact(XElement element)
        {
            return new Contact(element.Element("Id").Value)
            {
                Group = element.Element("Group").Value,
                Phone = element.Element("Phone").Value,
                MobilePhone = element.Element("MobilePhone").Value,
                Name = element.Element("Name").Value,
                Surname = element.Element("Surname").Value,
                Photo = convertStringToBitmap(element.Element("Photo").Value)
            };
        }

        /// <summary>
        /// Преобразовует картинку в строковое представление.
        /// </summary>
        /// <param name="img">Изображение для конвертации</param>
        /// <returns>Результат конвертации</returns>
        private string convertBitmapToString(Bitmap img)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            return Convert.ToBase64String(
                    (byte[])converter.ConvertTo(img, typeof(byte[])));
        }

        /// <summary>
        /// Восстанавливает картинку из строкового отображения.
        /// </summary>
        /// <param name="img">Закодированная строка</param>
        /// <returns>Полученное изоражение</returns>
        private Bitmap convertStringToBitmap(string img)
        {
            byte[] bytes = Convert.FromBase64String(img);
            using (var mem = new MemoryStream(bytes))
            {
                return new Bitmap(mem);
            }
        }
    }
}
