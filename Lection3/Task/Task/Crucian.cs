using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task
{
    /// <summary>
    /// Карась
    /// </summary>
    class Crucian : Fish, ILikeHay
    {

        public void EatHay()
        {
            Console.WriteLine("Ммм. Это не водорости. Попробую съесть это сено.");
        }
    }
}
