using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Task
{
    public class Employee : IXmlSerializable
    {
        private string EmployeeID;

        // по заданию необходимо было взять значение адреса из закрытого поля, 
        // поэтому для адреса нельзя было воспользоваться автоматически реализуемыми свойствами
        // так как поля, которые они создают могут менять свое название, а в коде нужно явно указывать
        // название поля.
        private string address; 

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int Age { get; set; }
        public string Department { get; set; }
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();
            // Получаем все открытые свойста данного класса
            foreach (var property in GetType().GetProperties())
            {
                // Считываем элемент из XML файла по имени свойства и преоразуем в тип этого свойства.
                object o = reader.ReadElementContentAs(property.PropertyType, null, property.Name, "");
                property.SetValue(this, o);
            }
            reader.ReadEndElement();
            EmployeeID = LastName + FirstName;

        }

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            // Получаем все открытые свойства данного класса
            foreach (var property in GetType().GetProperties())
            {
                // Записываем в  XML каждое своймтво, название такое же как и название свойства.
                writer.WriteElementString(property.Name, property.GetValue(this).ToString());
            }
        }
    }
}
