using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core
{
    public class ContactManager
    {
        private readonly XDocument contacts;

        public List<Contact> Contacts
        {
            get { return contacts.Elements().Cast<Contact>().ToList(); }
        } 

        public ContactManager(string path)
        {
            if (!File.Exists(path))
            {
                contacts = new XDocument();
                contacts.Save(path);
            }
            else
                contacts = XDocument.Load(path);
        }


    }
}
