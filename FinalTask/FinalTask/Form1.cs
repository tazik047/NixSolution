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
using System.Xml.Linq;

namespace FinalTask
{
    public partial class Form1 : Form
    {

        ConcurrentQueue<Item> xmlQueue = new ConcurrentQueue<Item>();
        ConcurrentQueue<Item> treeQueue = new ConcurrentQueue<Item>();

        private AutoResetEvent xmlWaitHamdler = new AutoResetEvent(false);
        private AutoResetEvent treeWaitHamdler = new AutoResetEvent(false);
        private ManualResetEvent waitStart = new ManualResetEvent(false);

        private ManualResetEvent[] waitWorker =
        {
            new ManualResetEvent(false),
            new ManualResetEvent(false)
        };

        private bool isProcessing = false;
        
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

        private void collectInfo(object obj)
        {
            var directory = new DirectoryInfo(path);
            waitStart.Set();
            try
            {
                long temp = 0;
                fillDirectory(directory, ref temp);
            }
            catch (UnauthorizedAccessException exception)
            {
                MessageBox.Show(exception.Message, Thread.CurrentThread.ManagedThreadId.ToString());
                this.NotifyException(ThreadException, exception);
            }
            waitStart.Reset();
            isProcessing = false;
            xmlWaitHamdler.Set();
            treeWaitHamdler.Set();
            WaitHandle.WaitAll(waitWorker);
            Invoke((Action)(() =>
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
            XElement doc = null;
            while (true)
            {
                while (!xmlQueue.IsEmpty)
                {
                    var item = xmlQueue.Dequeue();
                    if (item == null) continue;
                    doc = doc.WriteItem(item);
                }
                if (isProcessing)
                    xmlWaitHamdler.WaitOne();
                else
                {
                    if (doc != null)
                    {
                        doc.Save(pathXML);
                        doc = null;
                    }
                    waitWorker[0].Set();
                    waitStart.WaitOne();
                    waitWorker[0].Reset();
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
                            var newNode = new TreeNode(item.Name, 1, 2);
                            node.Nodes.Add(newNode);
                            node = newNode;
                            break;
                        case ItemType.EndFolder:
                            if (node.Parent != null)
                                node = node.Parent;
                            break;
                        case ItemType.File:
                            node.Nodes.Add(new TreeNode(item.Name, 0, 0));
                            break;
                    }
                }
                if (isProcessing)
                    treeWaitHamdler.WaitOne();
                else
                {
                    if (node != null && node.FirstNode != null)
                        BeginInvoke((Action)(() => treeView1.Nodes.Add(node.FirstNode)));
                    waitWorker[1].Set();
                    waitStart.WaitOne();
                    waitWorker[1].Reset();
                    BeginInvoke((Action)treeView1.Nodes.Clear);
                    node = new TreeNode();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                label1.Text = Path.GetDirectoryName(openFileDialog1.FileName);
                ThreadPool.QueueUserWorkItem(collectInfo);
                isProcessing = true;
                button1.Enabled = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var t = new ImageList();
            t.Images.Add(Properties.Resources.none);
            t.Images.Add(Properties.Resources.Folder_copy);
            t.Images.Add(Properties.Resources.Folder_copy_2);
            treeView1.ImageList = t;
            Thread xml = new Thread(XmlWorker);
            Thread tree = new Thread(TreeWorker);
            xml.IsBackground = true;
            tree.IsBackground = true;
            xml.Start();
            tree.Start();
        }

        void ThreadException(Exception exception)
        {
            MessageBox.Show(exception.Message, Thread.CurrentThread.ManagedThreadId.ToString());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (waitWorker.Any(w => !w.WaitOne(0)))
            {
                if (DialogResult.No ==
                    MessageBox.Show(
                        "Программа еще выполняет работу.\nЗавершение программы приведет к потере данных в xml файле.\nВы уверены, что хотите закрыть программу?",
                        "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    e.Cancel = true;
            }
        }
    }
}
