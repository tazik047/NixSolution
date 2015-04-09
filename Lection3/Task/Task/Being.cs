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
        public string BiengId { get; protected set; }

        /// <summary>
        /// Свойство только для чтения, которое возвращает количество
        /// ног конкретного существа.
        /// </summary>
        public abstract int CountLegs { get; }

        /// <summary>
        /// Этот констрктор будет вызываться при создании любого
        /// экземпляра производного от этого класса. Поэтому в нем
        /// можно определить общие действия для всех живых существ.
        /// </summary>
        public Being()
        {
            //Создаем уникальный идентификатор для каждом объекта.
            BiengId = Guid.NewGuid().ToString();
        }
    }
}
