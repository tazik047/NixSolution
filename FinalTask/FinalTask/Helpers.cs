using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FinalTask
{
    public static class Helpers
    {
        /// <summary>
        /// Метод расширения для записи элемента в дерево XML.
        /// </summary>
        /// <param name="writer">Текущий корень дерева</param>
        /// <param name="item">Элемент, который необходимо записать</param>
        /// <returns>Новый текущий корень</returns>
        public static XElement WriteItem(this XElement writer, Item item)
        {
            switch (item.Type)
            {
                case ItemType.StartFolder:
                    var t = new XElement("Folder", 
                        new XAttribute("Name", item.Name),
                        new XAttribute("Create", item.Created)
                        );
                    if (writer != null) // если элемент не первый, то добавляем его к текущему корню
                        writer.Add(t);
                    writer = t; // делаем новый элемент корнем
                    break;
                case ItemType.EndFolder:
                    //Имя может быть не null если произошла ошибка при просмотре этого узла,
                    //поэтому изменяем имя на новое, в котором указана ошибка
                    if (item.Name == null)
                        writer.Add(new XAttribute("Size", item.SizeMb)); // дозаписуем размер, который теперь известен
                    else
                        writer.SetAttributeValue("Name", item.Name);
                    if (writer.Parent != null) // если мы не поднялись до самого первого корня, то поднимаемся на один уровень вверх
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

        /// <summary>
        /// Метод расширения для записи элемента в дерево.
        /// </summary>
        /// <param name="node">Текущий корень дерева</param>
        /// <param name="item">Элемент, который необходимо записать</param>
        /// <returns>Новый текущий корень</returns>
        public static TreeNode WriteItem(this TreeNode node, Item item)
        {
            switch (item.Type)
            {
                case ItemType.StartFolder:
                    var newNode = new TreeNode(item.Name, 1, 2);
                    node.Nodes.Add(newNode);
                    node = newNode;// выбираем новую папку текущим корнем.
                    break;
                case ItemType.EndFolder:
                    //Имя может быть не null если произошла ошибка при просмотре этого узла,
                    //поэтому изменяем имя на новое, в котором указана ошибка.
                    if (item.Name != null)
                        node.Text = item.Name;
                    if (node.Parent != null) // если мы не поднялись до самого первого корня, то поднимаемся на один уровень вверх
                        node = node.Parent;
                    break;
                case ItemType.File:
                    node.Nodes.Add(new TreeNode(item.Name, 0, 0));
                    break;
            }
            return node;
        }

        /// <summary>
        /// Метод расширения для вывода сообщения об ошибках.
        /// </summary>
        /// <param name="invoke"></param>
        /// <param name="d">метод, который необходимо выполнить для обработки ошибки</param>
        /// <param name="ex">Исключение, необходимое обработать</param>
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