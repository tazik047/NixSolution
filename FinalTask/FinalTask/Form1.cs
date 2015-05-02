using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace FinalTask
{
    public partial class Form1 : Form
    {

        ConcurrentQueue<Item> xmlQueue = new ConcurrentQueue<Item>();
        ConcurrentQueue<Item> treeQueue = new ConcurrentQueue<Item>();

        private AutoResetEvent xmlWaitHamdler = new AutoResetEvent(false);
        private AutoResetEvent treeWaitHamdler = new AutoResetEvent(false);
        private ManualResetEvent waitStart = new ManualResetEvent(false);

        private AutoResetEvent[] waitWorker =
        {
            new AutoResetEvent(false),
            new AutoResetEvent(false)
        };

        private bool isProcessing = false;

        private Thread xml;
        private Thread tree;
        
        private string path
        {
            get { return label1.Text; }
        }

        private string pathXML
        {
            get { return "my.xml"; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void collectInfo()
        {
            var directory = new DirectoryInfo(path);
            long temp = 0;
            waitStart.Set();
            fillDirectory(directory, ref temp);
            waitStart.Reset();
            isProcessing = false;
            waitWorker[0].WaitOne();
            waitWorker[1].WaitOne();
            Invoke((Action) (() =>
            {
                button1.Enabled = true;
                MessageBox.Show("End work");
                isProcessing = false;
            }));
        }

        private long getInfo(DirectoryInfo directory)
        {
            long size = 0;
            foreach (var directoryInfo in directory.GetDirectories())
            {
                fillDirectory(directoryInfo, ref size);
            }

            foreach (var fileInfo in directory.GetFiles())
            {
                var item = new Item()
                {
                    Owner = directory.GetAccessControl().GetOwner(typeof (NTAccount)).ToString(),
                    Size = fileInfo.Length,
                    Type = ItemType.File
                };
                item.FillItem(fileInfo);
                addItemAndSetHandler(item);
                size += item.Size;
            }
            return size;
        }

        private void fillDirectory(DirectoryInfo directoryInfo, ref long size)
        {
            var item = new Item()
            {
                Owner = directoryInfo.GetAccessControl().GetOwner(typeof(NTAccount)).ToString(),
                Type = ItemType.StartFolder
            };
            item.FillItem(directoryInfo);
            addItemAndSetHandler(item);
            item = new Item()
            {
                Size = getInfo(directoryInfo),
                Type = ItemType.EndFolder
            };
            size += item.Size;
            addItemAndSetHandler(item);
        }

        private void addItemAndSetHandler(Item item)
        {
            xmlQueue.Enqueue(item);
            xmlWaitHamdler.Set();
            treeQueue.Enqueue(item);
            treeWaitHamdler.Set();
        }

        private void XmlWorker()
        {
            XmlWriter writer = null; 
            while (true)
            {
                while (!xmlQueue.IsEmpty)
                {
                    var item = xmlQueue.Dequeue();
                    if(item==null) continue;
                    writer.WriteItem(item);
                }
                if (isProcessing)
                    xmlWaitHamdler.WaitOne();
                else
                {
                    if (writer != null)
                    {
                        writer.Dispose();
                    }
                    waitWorker[0].Set();
                    waitStart.WaitOne();
                    waitWorker[0].Reset();
                    writer = new XmlTextWriter(pathXML, Encoding.UTF8);
                }
            }
        }

        private void TreeWorker()
        {
            TreeNode node = null;
            while (true)
            {
                while (!treeQueue.IsEmpty)
                {
                    var item = treeQueue.Dequeue();
                    if (item == null) continue;
                    switch (item.Type)
                    {
                        case ItemType.StartFolder:
                            var newNode = new TreeNode(item.Name);
                            node.Nodes.Add(newNode);
                            node = newNode;
                            break;
                        case ItemType.EndFolder:
                            if(node.Parent!=null)
                                node = node.Parent;
                            break;
                        case ItemType.File:
                            node.Nodes.Add(item.Name);
                            break;
                    }
                }
                if (isProcessing)
                    treeWaitHamdler.WaitOne();
                else
                {
                    if(node!=null)
                        Invoke((Action)(() => treeView1.Nodes.Add(node.FirstNode)));
                    waitWorker[1].Set();
                    waitStart.WaitOne();
                    waitWorker[1].Reset();
                    Invoke((Action)treeView1.Nodes.Clear);
                    node = new TreeNode();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                label1.Text = Path.GetDirectoryName(openFileDialog1.FileName);
                Thread collect = new Thread(collectInfo);
                isProcessing = true;
                button1.Enabled = false;
                collect.Start();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            xml = new Thread(XmlWorker);
            tree = new Thread(TreeWorker);
            xml.Start();
            tree.Start();
        }
    }
}
