using System;

namespace Task
{
    /// <summary>
    /// Плотва
    /// </summary>
    class Roach : Fish
    {
        public Roach()
        {
            BeingId = Guid.NewGuid().ToString();
        }
    }
}
