using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace TaskC
{
    class Program
    {
        //Создадим 1 общий на всю программу генератор случайных чисел.
        private static readonly Random rnd = new Random();

        static void Main(string[] args)
        {
            /* При работе со структурами (Int32) метод Add в ArrayList будет выполняться дольше,
             * так как этот метод в качестве параметров метода принимает Object(ссылочный тип).
             * Поэтому когда мы передаем этому методу структуру, то происходит упаковка(boxing) 
             * значимого типа в ссылочный. А при работе с List данной упаковки не происходит,
             * так как метод принимает тот тип, который задается при инициализации списка.
             * При работе с сылочными типами (String) существенного различия при работе с этими
             * двумя списками не наблюдается, так как для передачи ссылочных типов упаковка 
             * не требуется.
             * Операция boxing довольно таки дорогостоящая и поэтому ее нужно стараться избегать.
             */
            AddingTime<Int32>();
            AddingTime<String>();

            /* Из-за привидения типов при работе с ArrayList он уступает по производительности
             * списку List, который является строго типизированным. Благодря строгой типизации 
             * уменьшается количество возможно ошибок при привидении типов, а так же List не 
             * требует распаковки (unboxing) структур. Это позволяет List работать быстрее.
             */
            GettingTime<Int32>();
            GettingTime<String>();

            /* Сортировка структур в List так же проходит намного быстрее чем в ArrayList из-за
             * отсутствия упаковки и распаковки.
             */
            SortIntTime();
            SortStringTime();
        }

        static void AddingTime<T>()
        {
            Console.WriteLine("Compare List and ArrayList method Add with {0}:", typeof(T));
            int maxLoops = 10000;
            var list = new List<T>();
            var arrayList = new ArrayList();
            var listWatcher = new Stopwatch();
            var arrayListWatcher = new Stopwatch();

            listWatcher.Start();
            for (int i = 0; i < maxLoops; i++)
            {
                list.Add(default(T));
            }
            listWatcher.Stop();
            Console.WriteLine("Time for adding in List - {0}", listWatcher.ElapsedTicks);
            arrayListWatcher.Start();
            for (int i = 0; i < maxLoops; i++)
            {
                arrayList.Add(default(T));
            }
            arrayListWatcher.Stop();
            Console.WriteLine("Time for adding in ArrayList - {0}", arrayListWatcher.ElapsedTicks);
            Console.WriteLine();
        }

        static void GettingTime<T>()
        {
            Console.WriteLine("Compare List and ArrayList for getting {0} elements:", typeof(T));
            int maxLoops = 10000;
            var list = new List<T>();
            var arrayList = new ArrayList();
            var listWatcher = new Stopwatch();
            var arrayListWatcher = new Stopwatch();
            T temp; // В данную переменную будут записывать полученные значения из списка.

            //Генерация списков:
            for (int i = 0; i < maxLoops; i++)
            {
                list.Add(default(T));
                arrayList.Add(default(T));
            }

            listWatcher.Start();
            for (int i = 0; i < maxLoops; i++)
            {
                temp = list[i]; //получение элемента
            }
            listWatcher.Stop();
            Console.WriteLine("Time for getting in List - {0}", listWatcher.ElapsedTicks);
            arrayListWatcher.Start();
            for (int i = 0; i < maxLoops; i++)
            {
                // Необходимо приведение типов, так как ArrayList хранит внутри Object[]
                temp = (T)arrayList[i];
            }
            arrayListWatcher.Stop();
            Console.WriteLine("Time for getting in ArrayList - {0}", arrayListWatcher.ElapsedTicks);
            Console.WriteLine();
        }

        static void SortIntTime()
        {
            Console.WriteLine("Compare List and ArrayList sorting for System.Int32:");
            int maxLoops = 10000;
            var list = new List<Int32>();
            var arrayList = new ArrayList();
            var listWatcher = new Stopwatch();
            var arrayListWatcher = new Stopwatch();

            //Генерация списков:
            for (int i = 0; i < maxLoops; i++)
            {
                int temp = rnd.Next();
                list.Add(temp);
                arrayList.Add(temp);
            }

            listWatcher.Start();
            list.Sort();
            listWatcher.Stop();
            Console.WriteLine("Time for sorting List int - {0}", listWatcher.ElapsedTicks);
            arrayListWatcher.Start();
            arrayList.Sort();
            arrayListWatcher.Stop();
            Console.WriteLine("Time for sorting ArrayList int - {0}", arrayListWatcher.ElapsedTicks);
            Console.WriteLine();
        }

        static void SortStringTime()
        {
            Console.WriteLine("Compare List and ArrayList sorting for System.String:");
            int maxLoops = 10000;
            var list = new List<String>();
            var arrayList = new ArrayList();
            var listWatcher = new Stopwatch();
            var arrayListWatcher = new Stopwatch();

            //Генерация списков:
            for (int i = 0; i < maxLoops; i++)
            {
                String temp = GenerateRandomString();
                list.Add(temp);
                arrayList.Add(temp);
            }

            listWatcher.Start();
            list.Sort();
            listWatcher.Stop();
            Console.WriteLine("Time for sorting List strings - {0}", listWatcher.ElapsedTicks);
            arrayListWatcher.Start();
            arrayList.Sort();
            arrayListWatcher.Stop();
            Console.WriteLine("Time for sorting ArrayList strings - {0}", arrayListWatcher.ElapsedTicks);
            Console.WriteLine();
        }

        static String GenerateRandomString()
        {
            int length = 15; // длина генерируемого слова
            char[] ch = new char[length];
            for (int i = 0; i < length; i++)
            {
                ch[i] = (char)('a' + rnd.Next(26)); //генерирует случайную букву.
            }
            return new String(ch);
        }
    }
}
