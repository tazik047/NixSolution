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
        public void Breathe()
        {
            Console.WriteLine("Дышу зябрами.");
        }
    }
}
