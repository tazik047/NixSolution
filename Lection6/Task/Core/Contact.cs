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
        public string Id { get; private set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public Bitmap Photo { get; set; }

        public Contact()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Contact(String id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return Surname + " " + Name;
        }
    }
}
