using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task
{
    /// <summary>
    /// Интерфейс, которые должны реализовать существа, которые едят сено.
    /// </summary>
    interface ILikeHay
    {
        /// <summary>
        /// Есть сено.
        /// </summary>
        void EatHay();
    }
}
