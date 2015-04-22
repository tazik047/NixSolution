using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core
{
    public class Contact
    {
        private string id;
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public Bitmap Photo { get; set; }

        public static explicit operator Contact(XElement element)
        {
            return new Contact();
        }
    }
}
