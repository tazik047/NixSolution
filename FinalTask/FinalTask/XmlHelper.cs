using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FinalTask
{
    public static class XmlHelper
    {
        public static void WriteItem(this XmlWriter writer, Item item)
        {
            switch (item.Type)
            {
                case ItemType.StartFolder:
                    writer.WriteStartElement("Folder");
                    writePartOfItem(writer, item);
                    break;
                case ItemType.EndFolder:
                    double mb = item.Size / (1024.0 * 1024);
                    //writer.WriteAttributeString("Size", string.Format("{0} MB", mb));
                    writer.WriteEndElement();
                    break;
                case ItemType.File:
                    writer.WriteStartElement("File");
                    writePartOfItem(writer, item);
                    writer.WriteEndElement();
                    break;
            }
        }

        private static void writePartOfItem(XmlWriter writer, Item item)
        {
            writer.WriteAttributeString("Name", item.Name);
            writer.WriteAttributeString("Path", item.Path);
            writer.WriteAttributeString("Create", string.Format("{0} {1}", 
                item.CreationDate.ToShortDateString(),
                item.CreationDate.ToShortTimeString()));
            writer.WriteAttributeString("LastModify", string.Format("{0} {1}", 
                item.LastModifiedDate.ToShortDateString(), 
                item.LastModifiedDate.ToShortTimeString()));
            writer.WriteAttributeString("LastAccess", string.Format("{0} {1}", 
                item.LastAccessDate.ToShortDateString(), 
                item.LastAccessDate.ToShortTimeString()));
            writer.WriteAttributeString("Attrubtes", item.Attributes.ToString());
            writer.WriteAttributeString("Owner", item.Owner);
            writer.WriteAttributeString("Rights", item.Rights);
        }
    }
}