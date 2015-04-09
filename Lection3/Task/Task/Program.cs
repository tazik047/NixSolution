using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task
{
    class Program
    {
        static void Main(string[] args)
        {
            var collection = new List<Being>{
                new Roach(),
                new Horse(),
                new Dog(),
                new Crucian(),
                new Dog(),
                new Horse(),
                new Roach(),
                new Dog()
            };

            Console.WriteLine("Количество ног у животных: {0}", CountLegs(collection)); // 20 = 3 собаки + 2 лошади
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("Существа, которые могут дышать под водой:");
            CanBreatheUnderwater(collection);
        }

        //Считает количество ног у последовательности живых существ.
        static int CountLegs(IEnumerable<Being> beings)
        {
            return beings.Sum(b => b.CountLegs);
        }

        //Выводит ID существ, которые могут дышать под водой.
        static void CanBreatheUnderwater(IEnumerable<Being> beings)
        {
            foreach (var i in beings)
            {
                //Если объект реализует интерфейс IUnderwater, то он может дышать под водой
                if (i is IUnderwater)
                    Console.WriteLine("\tТип животного: {0},\t id = {1}", i.GetType().Name, i.BiengId);
            }
        }
    }
}
