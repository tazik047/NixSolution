using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task
{
    /// <summary>
    /// Собака
    /// </summary>
    class Dog : Animal, IUnderwater
    {
        public override int CountLegs
        {
            get { return 4; }
        }

        public void Breathe()
        {
            Console.WriteLine("Я задыхаюсь, не могу дышать под водой.");
        }
    }
}
