using System;

namespace Task
{
    /// <summary>
    /// Карась
    /// </summary>
    class Crucian : Fish, ILikeHay
    {
        public Crucian()
        {
            BeingId = Guid.NewGuid().ToString();
        }

        public void EatHay()
        {
            Console.WriteLine("Ммм. Это не водорости. Попробую съесть это сено.");
        }
    }
}
