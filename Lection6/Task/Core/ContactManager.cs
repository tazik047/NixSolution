using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Core
{
    public class ContactManager
    {
        private readonly XDocument contacts;
        private readonly string path;

        public List<Contact> Contacts
        {
            get { return contacts.Root.Elements().Select(getContact).ToList(); }
        }

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
            this.path = path;
            if (!File.Exists(path))
            {
                contacts = new XDocument(new XElement("Contacts"));
                contacts.Save(path);
            }
            else
                contacts = XDocument.Load(path);
        }

        public List<Contact> SearchContactList(string name)
        {
            var names = name.Split().Where(s=>!string.IsNullOrWhiteSpace(s)).Select(s=>s.ToUpper());
            if (!names.Any()) return Contacts;

            return contacts.Root.Elements()
                .Where(e => names.All(
                    name1 => e.Element("Name").Value.ToUpper().Contains(name1) 
                        || e.Element("Surname").Value.ToUpper().Contains(name1)
                    ))
                .Select(getContact).ToList();
        }

        public Dictionary<string, List<Contact>> SearchContactDictionary(string name)
        {
            return SearchContactList(name).GroupBy(c => c.Group)
                .Select(gr => gr.ToList())
                .ToDictionary(gr => gr[0].Group);
        } 

        public Contact GetContactById(string id)
        {
            return getContact(contacts.Root
                                    .Elements().First(e => e.Element("Id")
                                    .Value.Equals(id)));
        }

        public void Add(Contact contact)
        {
            contacts.Root.Add(getXElement(contact));
            contacts.Save(path);
        }

        public void Remove(string id)
        {
            contacts.Root.Elements().First(e=>e.Element("Id").Value.Equals(id)).Remove();
            contacts.Save(path);
        }

        public void Update(Contact contact)
        {
            var current = contacts.Root.Elements().First(e => e.Element("Id").Value.Equals(contact.Id));
            current.ReplaceWith(getXElement(contact));
            contacts.Save(path);
        }

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

        private string convertBitmapToString(Bitmap img)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
            return Convert.ToBase64String(
                    (byte[])converter.ConvertTo(img, typeof(byte[])));
        }

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
