using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskD
{
    class Program
    {
        static void Main(string[] args)
        {
            List<MyGeneric<int>> list = new List<MyGeneric<int>>();
            //List<MyGeneric<string> > list1 = new List<MyGeneric<string>>(); - выдает ошибку компиляции из-за задоного ограничения на тип Т
            var t = Comparer<MyGeneric<int>>.Default;
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new MyGeneric<int>(rnd.Next(100)));
            }
            Print(list);

            /* Если бы классом MyGeneric<T> не был реализован интерфейс IComparable<MyGeneric<T>>,
             * то во время сортировки возникало бы исключение, так как метод сортировки не знал бы
             * как сравнивать экзмепляры данного класса.
             */
            list.Sort();
            Print(list);
        }

        static void Print<T>(List<T> list)
        {
            list.ForEach(e => Console.Write("{0,2}, ", e)); // Распечатывает список элементов через пробел.
            Console.WriteLine();
        }
    }
}
