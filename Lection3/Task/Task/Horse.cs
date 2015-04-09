using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task
{
    /// <summary>
    /// Лошадь
    /// </summary>
    class Horse : Animal, ILikeHay
    {
        public override int CountLegs
        {
            get { return 4; }
        }

        public void EatHay()
        {
            Console.WriteLine("О. Опять сено. Люблю есть сено.");
        }
    }
}
