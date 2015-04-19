using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task
{
    public partial class Form1 : Form
    {
        //Массив возможных исключение.
        private readonly Type[] exceptions = { typeof(IndexOutOfRangeException), 
                                                 typeof(System.IO.FileNotFoundException), 
                                                 typeof(NullReferenceException), 
                                                 typeof(OutOfMemoryException), 
                                                 typeof(ArgumentException), 
                                                 typeof(InvalidCastException), 
                                                 typeof(StackOverflowException) };

        private object locker = new object();

        private Queue<Exception> queueExceptions;

        private volatile bool work = true;

        private ManualResetEvent workEvent;


        public Form1()
        {
            InitializeComponent();
            queueExceptions = new Queue<Exception>();
            workEvent = new ManualResetEvent(false);
            Thread thread1 = new Thread(ThreadWorker);
            Thread thread2 = new Thread(ThreadWorker);
            thread1.Name = "Первый поток";
            thread2.Name = "Второй поток";
            thread1.IsBackground = true;  // Делаем потоки фоновыми для того, чтобы они завершались
            thread2.IsBackground = true;  // при завершении основного потока. 
            thread1.Start();
            thread2.Start();
        }

        public void ThreadWorker()
        {
            string nameOfThread = Thread.CurrentThread.Name;
            // Для каждого потока будет разное начальное состояние рандома.
            Random rnd = new Random(Thread.CurrentThread.ManagedThreadId + Environment.TickCount);

            Type[] constructorTypes = { typeof(String) };
            object[] paramsToConstructor = { nameOfThread };

            while (true)
            {
                while (work)
                {
                    try
                    {
                        Thread.Sleep(rnd.Next(1000, 10000)); // Выполнение какой-то работы в течении 1-10 сек.
                        // Не выбрасываем исключение если была нажата кнопка приостановки потока, пока поток спал
                        if (!work) break;
                        int index = rnd.Next(exceptions.Length);
                        // Используем отражение для того, чтобы создать экземпялр одного из классов исключений.
                        Exception e = (Exception)exceptions[index]
                            .GetConstructor(constructorTypes)
                            .Invoke(paramsToConstructor);
                        throw e;
                    }

                    catch (Exception ex)
                    {
                        //блокируем доступ к очереди пока с ней работает другой поток.
                        lock (locker)
                            queueExceptions.Enqueue(ex);
                        // Уведомляем главный поток о появлении исключения.
                        Invoke((Action)ExceptionHandler);
                    }
                }
                //Ожидаем сигнала из главного потока о возобновлении работы потока
                workEvent.WaitOne();
            }
        }

        private void ExceptionHandler()
        {
            Exception e;
            lock (locker)
                e = queueExceptions.Dequeue();

            richTextBox1.AppendText("В потоке ");
            richTextBox1.AppendBoldText(string.Format("\"{0}\"", e.Message));
            richTextBox1.AppendText(" произошло исключение: ");
            richTextBox1.AppendBoldTextWithColor(e.GetType().Name, Color.Red);
            richTextBox1.AppendText("." + Environment.NewLine);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            work = !work;
            button1.Text = work ? "Остановить генерацию ошибок" : "Возобновить генерацию ошибок";
            if (work)
                workEvent.Set(); // Сигнализируем потокам о возобновлении работы.
            else
                workEvent.Reset(); // Блокируем потоки до ожидания сигнала о возобновлении их работы.
            
        }
    }
}
