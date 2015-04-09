using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task
{
    /// <summary>
    /// Рыба
    /// </summary>
    abstract class Fish : Being, IUnderwater
    {
        // У всех рыб нет ног.
        public override int CountLegs
        {
            get { return 0; }
        }

        public void Breathe()
        {
            Console.WriteLine("Дышу зябрами.");
        }
    }
}
