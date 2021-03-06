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
        /// <summary>
        /// Очередь на обработку элементов для xml.
        /// </summary>
        readonly MyConcurrentQueue<Item> _xmlQueue = new MyConcurrentQueue<Item>();
        
        /// <summary>
        /// Очередь на обработку элементов для tree.
        /// </summary>
        readonly MyConcurrentQueue<Item> _treeQueue = new MyConcurrentQueue<Item>();

        /// <summary>
        /// WaitHandler сигнализирующий о появлении элемента в очереди для xml.
        /// </summary>
        private readonly AutoResetEvent _xmlWaitHamdler = new AutoResetEvent(false);

        /// <summary>
        /// WaitHandler сигнализирующий о появлении элемента в очереди для tree.
        /// </summary>
        private readonly AutoResetEvent _treeWaitHamdler = new AutoResetEvent(false);
        
        /// <summary>
        /// WaitHandler сигнализирующий о начале работы алгоритма.
        /// </summary>
        private readonly ManualResetEvent _waitStart = new ManualResetEvent(false);

        /// <summary>
        /// WaitHandler для сигнализации об начале или окончании работы потоков. 
        /// </summary>
        private readonly ManualResetEvent[] _waitWorker =
        {
            new ManualResetEvent(false), // для потока, работающег с xml
            new ManualResetEvent(false)  // для потока, работающег с tree
        };

        /// <summary>
        /// Массив конопок для работы с формой.
        /// </summary>
        private readonly Button[] _buttons;

        /// <summary>
        /// Показаывает выполняется ли работа.
        /// </summary>
        private bool _isProcessing = false;

        /// <summary>
        /// Форма для отображения ошибок.
        /// </summary>
        private ErrorForm _error;

        /// <summary>
        /// Путь к папке, которую необходимо просмотреть.
        /// </summary>
        private string path
        {
            get { return label1.Text; }
        }

        /// <summary>
        /// Пуьб к файлу, в котором необходимо сохранить результат в xml.
        /// </summary>
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

        /// <summary>
        /// Метод для сбора информации об указанной папке
        /// </summary>
        private void collectInfo(object obj)
        {
            var directory = new DirectoryInfo(path);
            _waitStart.Set(); // Сигнализируем другим потокам о начале работы.
            long temp = 0;
            fillDirectory(directory, ref temp);
            _waitStart.Reset(); // Сигнализируем об окончании работы.
            _isProcessing = false;
            _xmlWaitHamdler.Set(); // Сигнализируем потокам, если они ждут
            _treeWaitHamdler.Set(); // новые элементы.
            WaitHandle.WaitAll(_waitWorker); // Дожидаемся окончания работы всех потоков.
            Invoke((Action)(() =>
            {
                enableOrDisableButtons(true);
                if (_error == null || !_error.HaveError)
                    MessageBox.Show("Процесс просмотра папок завершен успешно.");
                else
                {
                    MessageBox.Show("Процесс просмотра папок завершился с ошибками");
                    errorButton.Visible = true;
                }
                _isProcessing = false;
            }));
        }

        /// <summary>
        /// Получает всю информацию о папке.
        /// </summary>
        /// <param name="directory">Папка, информацию о которой необходимо получить.</param>
        /// <returns>Размер папки</returns>
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

        /// <summary>
        /// Метод для добавления информации о папке и всем что с ней связано в очередь.
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="size"></param>
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
            catch (DirectoryNotFoundException e)
            {
                this.NotifyException(threadException, e);
                item = new Item()
                {
                    Name = directoryInfo.Name + " (путь не найден)",
                    Type = ItemType.EndFolder
                };
            }
            catch (UnauthorizedAccessException e)
            {
                this.NotifyException(threadException, e);
                item = new Item()
                {
                    Name = directoryInfo.Name + " (нет доступа)",
                    Type = ItemType.EndFolder
                };
            }
            catch (System.Security.SecurityException e)
            {
                this.NotifyException(threadException, e);
                item = new Item()
                {
                    Name = directoryInfo.Name + " (нет разрешений для доступа)",
                    Type = ItemType.EndFolder
                };
            }
            catch (IOException e)
            {
                this.NotifyException(threadException, e);
                item = new Item()
                {
                    Name = directoryInfo.Name + " (ошибка чтения)",
                    Type = ItemType.EndFolder
                };
            }
            addItemAndSetHandler(item);
        }

        /// <summary>
        /// Вспомогательный метод для добавления элемента в очереди и сигнализации об этом.
        /// </summary>
        /// <param name="item">Элемент для добавления.</param>
        private void addItemAndSetHandler(Item item)
        {
            _xmlQueue.Enqueue(item); // Добавляем элемент в очередь для xml
            _xmlWaitHamdler.Set(); // и сигнализируем, что в очереди появился элемент.
            _treeQueue.Enqueue(item); // добавляем элемент в очередь для дерева
            _treeWaitHamdler.Set(); // и сигнализируем, что в очереди появился элемент.
        }

        /// <summary>
        /// Метод для обработки элементов и помещения их в xml дерево.
        /// </summary>
        private void xmlWorker()
        {
            XElement doc = null;
            while (true) //работает все время жизни приложения
            {
                while (!_xmlQueue.IsEmpty)
                {
                    var item = _xmlQueue.Dequeue();
                    if (item == null) continue;
                    doc = doc.WriteItem(item);
                }
                if (_isProcessing) // пока еще есть работа
                    _xmlWaitHamdler.WaitOne(); // ожидаем пока в очереди не появится новый элемент
                else // иначе работа окончена
                {
                    if (doc != null) // если была какая-либо работа, то сохраняем все в xml файл. 
                    {
                        doc.Save(pathXML);
                        doc = null;
                    }
                    _waitWorker[0].Set(); // Сигнализируем, что метод закончил свою работу.
                    _waitStart.WaitOne(); // Ожидаем новой работы.
                    _waitWorker[0].Reset(); // Сигнализируем, что метод начал работать.
                }
            }
        }

        /// <summary>
        /// Метод для обработки элементов и помещения их в дерево.
        /// </summary>
        private void treeWorker()
        {
            TreeNode node = null;
            while (true) //работает все время жизни приложения
            {
                while (!_treeQueue.IsEmpty)
                {
                    var item = _treeQueue.Dequeue();
                    if (item == null) continue;
                    node = node.WriteItem(item);
                }
                if (_isProcessing) // пока еще есть работа
                    _treeWaitHamdler.WaitOne(); // ожидаем пока в очереди не появится новый элемент
                else // иначе работа окончена
                {
                    if (node != null) // если была какая-либо работа, то добавляем все дерево на форму. 
                        BeginInvoke((Action)(() => treeView1.Nodes.Add(node.FirstNode)));
                    _waitWorker[1].Set(); // Сигнализируем, что метод закончил свою работу.
                    _waitStart.WaitOne(); // Ожидаем новой работы.
                    _waitWorker[1].Reset(); // Сигнализируем, что метод начал работать.
                    BeginInvoke((Action)treeView1.Nodes.Clear); // Очищаем форму
                    node = new TreeNode(); // и создаем новое дерево.
                }
            }
        }

        /// <summary>
        /// Запускает работы программы
        /// </summary>
        private void start_button_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(label1.Text)) // если не была выбрана папка, то:
            {
                MessageBox.Show("Сначала нужно выбрать папку");
                return;
            }
            if (_error != null) // если в прошлый раз были ошибки, то очищаем форму.
                _error.ClearError();
            errorButton.Visible = false; // скрываем кнопку показа ошибок
            ThreadPool.QueueUserWorkItem(collectInfo); // Ставим основной метод в пул потоков.
            _isProcessing = true;
            enableOrDisableButtons(false);
        }

        /// <summary>
        /// Включает или отключает кнопки для работы с формой.
        /// </summary>
        /// <param name="enable">true - включить
        /// false - выключить</param>
        private void enableOrDisableButtons(bool enable)
        {
            foreach (var button in _buttons)
                button.Enabled = enable;
        }

        /// <summary>
        /// Обработчик события загрузки формы. Подготавливает потоки для работы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            var t = new ImageList();
            t.Images.Add(Properties.Resources.none);
            t.Images.Add(Properties.Resources.Folder_copy);
            t.Images.Add(Properties.Resources.Folder_copy_2);
            treeView1.ImageList = t;
            Thread xml = new Thread(xmlWorker);
            Thread tree = new Thread(treeWorker);
            xml.IsBackground = true; //делаем потоки фоновые для того, чтобы они закрывались
            tree.IsBackground = true; // вместе с приложением.
            xml.Start();  //запускаем потоки
            tree.Start(); //запускаем потоки
        }

        /// <summary>
        /// Добавляет исключение на форму с ошибками.
        /// </summary>
        /// <param name="exception">Исключение, которое необходимо добавить</param>
        void threadException(Exception exception)
        {
            if (_error == null) // если еще не создавали форму то создадим ее.
                _error = new ErrorForm();
            _error.AddError(exception.Message);
        }

        /// <summary>
        /// Обработчи события перед закрытием формы.
        /// Предостерегает о том, что приложение может закрыться пока еще идет работа.
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_waitWorker.Any(w => !w.WaitOne(0))) // если хотя бы 1 из потокв еще не освободился, то:
            {
                if (DialogResult.No ==
                    MessageBox.Show(
                        "Программа еще выполняет работу.\nЗавершение программы приведет к потере данных в xml файле.\nВы уверены, что хотите закрыть программу?",
                        "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    e.Cancel = true;
            }
        }

        /// <summary>
        /// Отображает диалоговое окно с выбором директории для работы с ней.
        /// </summary>
        private void pathButton_Click(object sender, EventArgs e)
        {
            var browser = new FolderBrowserDialog {ShowNewFolderButton = false};
            if (browser.ShowDialog() == DialogResult.OK)
                label1.Text = browser.SelectedPath;
        }

        /// <summary>
        /// Отображает диалоговое окно для выбора файла для сохранения xml результата.
        /// </summary>
        private void xmlButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == saveFileDialog1.ShowDialog())
                label2.Text = saveFileDialog1.FileName;
        }

        /// <summary>
        /// Показывает окно с ошибками.
        /// </summary>
        private void errorButton_Click(object sender, EventArgs e)
        {
            _error.Show();
        }
    }
}
