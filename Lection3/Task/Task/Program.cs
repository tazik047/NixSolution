using System;
using System.Collections.Generic;
using System.Linq;

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

        static int CountLegs(IEnumerable<Being> beings)
        {
            return beings.OfType<Animal>().Sum(a => a.CountLegs);
        }

        // Displays ID beings who can breathe underwater.
        static void CanBreatheUnderwater(IEnumerable<Being> beings)
        {
            foreach (var i in beings)
            {
                // If a being implement IUnderwater, than it can breathe underwater.
                if (i is IUnderwater)
                    Console.WriteLine("\tТип животного: {0},\t id = {1}", i.GetType().Name, i.BeingId);
            }
        }
    }
}
