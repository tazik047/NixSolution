using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TaskB
{
    class Program
    {
        static int[] nums = { 12, 5, 16, 3, 4, 7, 45, 2, 65, 70, 80, 1, 0 };

        static void Main(string[] args)
        {

            WorkWithMyList();
            WorkWthList();
            WorkWthSortedList();
            /* При количестве элементов меньше 6 MyGenericList имеет доступ к элементам со схожей
             * скоростью как List(это логично, так как он в данный момент использует List). Так же
             * на небольшом количестве элементов SortedList получает элементы более медленнее, чем
             * List. Но при возрастании количества элементов время доступа к ним в List выросло с 
             * 6569 тактов до 16290, а вот время доступа к элементам SortedList изменилось не
             * существенно: с 9684 до 15804. То есть SortedList лучше работает ну большем обьеме
             * данных. Так получается потому что доступ к элементам SortedList происходит за O(log n),
             * а к элементам списка за O(n). Точно так же доступ к элементам MyGenericList при их
             * количестве больше 5 становится схожим на SortedList.
             * При сравнении добавления элементов в мою коллекцию и в коллекции List и SortedList
             * по времени получалась существенная разница. Это связано с тем, что при подсчете
             * времени добавления учитывалась та ситуация, когда необходимо было перейти на
             * использование другого списка, именно из-за этого потреблялось существенно больше
             * времени. Так же при добавлении элементы проверяются на уникальность(SortedList 
             * запрещает использование одиннаковых ключей), это так же занимает некоторое время.
             */
        }

        static void WorkWithMyList()
        {
            var myList = new MyGenericList<int>();
            var myGenericTimer = new Stopwatch();
            myGenericTimer.Start();
            for (int i = 0; i < 5; i++)
                myList.Add(nums[i]);
            myGenericTimer.Stop(); // Приостанавливает таймер, пока распечатывается список.
            printList(myList);
            myGenericTimer.Start(); // Возобновляет таймер.
            for (int i = 6; i < nums.Length; i++)
                myList.Add(nums[i]);
            myGenericTimer.Stop();
            printList(myList);
            Console.WriteLine("Time for work with MyGenericList: {0}\n",
                myGenericTimer.ElapsedTicks);
        }

        static void WorkWthList()
        {
            var list = new List<int>();
            var listTimer = new Stopwatch();
            listTimer.Start();
            for (int i = 0; i < 5; i++)
                list.Add(nums[i]);
            listTimer.Stop(); // Пауза таймера.
            printList(list);
            listTimer.Start(); // Возобновление таймера.
            for (int i = 6; i < nums.Length; i++)
                list.Add(nums[i]);
            listTimer.Stop();
            printList(list);
            Console.WriteLine("Time for work with List: {0}\n",
                listTimer.ElapsedTicks);
        }

        static void WorkWthSortedList()
        {
            var list = new SortedList<int, int>();
            var sortedListTimer = new Stopwatch();
            sortedListTimer.Start();
            for (int i = 0; i < 5; i++)
                list.Add(nums[i], nums[i]);
            sortedListTimer.Stop(); // Пауза таймера.
            printList(list);
            sortedListTimer.Start(); // Возобновление таймера.
            for (int i = 6; i < nums.Length; i++)
                list.Add(nums[i], nums[i]);
            sortedListTimer.Stop();
            printList(list);
            Console.WriteLine("Time for work with SortedList: {0}\n",
                sortedListTimer.ElapsedTicks);
        }

        private static void printList<T>(IEnumerable<T> list)
        {
            Console.WriteLine("Print {0}:", list.GetType().Name);
            int index = 0;
            var timer = new Stopwatch();
            timer.Start();
            foreach (var e in list)
            {
                Console.WriteLine("\tAt {0} is element: {1}", index++, e);
            }
            timer.Stop();
            Console.WriteLine("Time for getting elements: {0}", timer.ElapsedTicks);
            Console.WriteLine();
        }
    }
}
