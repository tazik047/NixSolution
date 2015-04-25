using System;
using System.Drawing;

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
