using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task
{
    /// <summary>
    /// Интерфейс, который должны реализовать существа, которые дышат под водой.
    /// </summary>
    interface IUnderwater
    {
        /// <summary>
        /// Дышать под водой.
        /// </summary>
        void Breathe();
    }
}
