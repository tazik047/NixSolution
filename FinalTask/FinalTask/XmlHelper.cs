using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FinalTask
{
    public static class XmlHelper
    {
        public static XElement WriteItem(this XElement writer, Item item)
        {
            switch (item.Type)
            {
                case ItemType.StartFolder:
                    var t = new XElement("Folder");
                    if(writer != null)
                        writer.Add(t);
                    writer = t;
                    writer.Add(new XAttribute("Name", item.Name),
                        new XAttribute("Create", item.Created));
                    break;
                case ItemType.EndFolder:
                    writer.Add(new XAttribute("Size", item.SizeMb));
                    if(writer.Parent != null)
                        writer = writer.Parent;
                    break;
                case ItemType.File:
                    double size = item.Size / (1024.0 * 1024);
                    writer.Add(new XElement("File",
                        new XAttribute("Name", item.Name),
                        new XAttribute("Create", item.Created),
                        new XAttribute("Size",  item.SizeMb))
                        );
                    break;
            }
            return writer;
        }
    }
}