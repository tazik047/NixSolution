using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task
{
    /// <summary>
    /// Живое существо
    /// </summary>
    public abstract class Being
    {
        /// <summary>
        /// Идентификатор существа, который доступен для чтения, 
        /// а для записи доступен только для производных классов.
        /// </summary>
        public string BeingId { get; protected set; }
    }
}
