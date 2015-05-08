﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
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

        private Button[] _buttons;

        private bool isProcessing = false;

        private ErrorForm error;

        private string path
        {
            get { return label1.Text; }
        }

        private string pathXML
        {
            get { return label2.Text; }
        }

        public Form1()
        {
            InitializeComponent();
            _buttons = new[]
            {
                startButton, pathButton, xmlButton
            };
        }

        private void collectInfo(object obj)
        {
            var directory = new DirectoryInfo(path);
            waitStart.Set();
            long temp = 0;
            fillDirectory(directory, ref temp);
            waitStart.Reset();
            isProcessing = false;
            xmlWaitHamdler.Set();
            treeWaitHamdler.Set();
            WaitHandle.WaitAll(waitWorker);
            Invoke((Action)(() =>
            {
                enableOrDisableButtons(true);
                if (error == null || !error.HaveError)
                    MessageBox.Show("Процесс просмотра папок завершен успешно.");
                else
                {
                    MessageBox.Show("Процесс просмотра папок завершился с ошибками");
                    button1.Visible = true;
                }
                isProcessing = false;
            }));
        }

        private long getInfo(DirectoryInfo directory)
        {
            long size = 0;
            foreach (var directoryInfo in directory.GetDirectories())
            {
                //try
                //{
                    fillDirectory(directoryInfo, ref size);
                //}
                //catch (UnauthorizedAccessException exception)
                //{
                //    this.NotifyException(ThreadException, exception);
                //}
                //catch (DirectoryNotFoundException exception)
                //{
                //    this.NotifyException(ThreadException, exception);
                //}
                //catch (System.Security.SecurityException exception)
                //{
                //    this.NotifyException(ThreadException, exception);
                //}
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
            try
            {
                item = new Item()
                {
                    Size = getInfo(directoryInfo),
                    Type = ItemType.EndFolder
                };
                size += item.Size;
            }
            catch (Exception e)
            {
                this.NotifyException(ThreadException, e);
                item = new Item()
                {
                    Name = directoryInfo.Name + " (нет доступа)",
                    Type = ItemType.EndFolder
                };
            }
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
                    node = node.WriteItem(item);
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

        private void start_button_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(label1.Text))
            {
                MessageBox.Show("Сначала нужно выбрать папку");
                return;
            }
            if(error !=null)
                error.ClearError();
            button1.Visible = false;
            ThreadPool.QueueUserWorkItem(collectInfo);
            isProcessing = true;
            enableOrDisableButtons(false);
        }

        private void enableOrDisableButtons(bool enable)
        {
            foreach (var button in _buttons)
                button.Enabled = enable;
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
            if(error == null)
                error = new ErrorForm();
            error.AddError(exception.Message);
            //MessageBox.Show(exception.Message, Thread.CurrentThread.ManagedThreadId.ToString());
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

        private void pathButton_Click(object sender, EventArgs e)
        {
            var browser = new FolderBrowserDialog();
            if(browser.ShowDialog() == DialogResult.OK)
                label1.Text = browser.SelectedPath;
        }

        private void XmlButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == saveFileDialog1.ShowDialog())
                label2.Text = saveFileDialog1.FileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            error.Show();
        }
    }
}
