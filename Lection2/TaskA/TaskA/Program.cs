using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskA
{
    class Program
    {
        static void Main(string[] args)
        {
            FunWithFor();
            //FunWithDoWhile();
            //FunWithWhile();
            //FunWithForeach();
        }

        static void FunWithFor()
        {
            Console.WriteLine("\nFun With For");
            for (int i = 1; i <= 10; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    WriteNumber(i, j);
                }
                Console.WriteLine();
            }
        }

        static void FunWithDoWhile()
        {
            Console.WriteLine("\nFun with do while");
            int i = 1;
            do
            {
                int j = 1;
                do
                {
                    WriteNumber(i, j);
                    j++;
                } while (j <= 10);
                Console.WriteLine();
                i++;
            } while (i <= 10);
        }

        static void FunWithWhile()
        {
            Console.WriteLine("\nFun with while");
            int i = 1;
            while (i <= 10)
            {
                int j = 1;
                while (j <= 10)
                {
                    WriteNumber(i, j);
                    j++;
                }
                Console.WriteLine();
                i++;
            }
        }

        static void FunWithForeach()
        {
            Console.WriteLine("\nFun with foreach");
            foreach (int i in Enumerable.Range(1, 10))
            {
                foreach (int j in Enumerable.Range(1, 10))
                {
                    WriteNumber(i, j);
                }
                Console.WriteLine();
            }
        }

        private static void WriteNumber(int i, int j)
        {
            if (i == j)
            {
                ConsoleColor defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("{0,3} ", i * j);
                Console.ForegroundColor = defaultColor;
                return;
            }
            // Дополняет число ведущими нулями до 3 знаков (002).
            // Console.Write("{0:D3} ", i * j); 

            // Выделяет под запись числа 3 знака, если число содержит меньше знаков, то прижимает его к левому краю.
            // Console.Write("{0,-3} ", i * j);

            // Выполняет тоже, что и предыдущая строка, только прижимает текст к правому краю.
            Console.Write("{0,3} ", i * j);
        }
    }
}
