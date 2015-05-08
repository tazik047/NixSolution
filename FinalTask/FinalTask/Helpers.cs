using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FinalTask
{
    public static class Helpers
    {
        public static XElement WriteItem(this XElement writer, Item item)
        {
            switch (item.Type)
            {
                case ItemType.StartFolder:
                    var t = new XElement("Folder");
                    if (writer != null)
                        writer.Add(t);
                    writer = t;
                    writer.Add(new XAttribute("Name", item.Name),
                        new XAttribute("Create", item.Created));
                    break;
                case ItemType.EndFolder:
                    if (item.Name == null)
                        writer.Add(new XAttribute("Size", item.SizeMb));
                    else
                        writer.SetAttributeValue("Name", item.Name);
                    if (writer.Parent != null)
                        writer = writer.Parent;
                    break;
                case ItemType.File:
                    writer.Add(new XElement("File",
                        new XAttribute("Name", item.Name),
                        new XAttribute("Create", item.Created),
                        new XAttribute("Size", item.SizeMb))
                        );
                    break;
            }
            return writer;
        }

        public static TreeNode WriteItem(this TreeNode node, Item item)
        {
            switch (item.Type)
            {
                case ItemType.StartFolder:
                    var newNode = new TreeNode(item.Name, 1, 2);
                    node.Nodes.Add(newNode);
                    node = newNode;
                    break;
                case ItemType.EndFolder:
                    if (item.Name != null)
                        node.Text = item.Name;
                    if (node.Parent != null)
                        node = node.Parent;
                    break;
                case ItemType.File:
                    node.Nodes.Add(new TreeNode(item.Name, 0, 0));
                    break;
            }
            return node;
        }

        public static void NotifyException(this ISynchronizeInvoke invoke,
            Action<Exception> d, Exception ex)
        {
            if (!invoke.InvokeRequired)
                d(ex);
            else
                invoke.BeginInvoke(d, new[] { ex });
        }
    }
}