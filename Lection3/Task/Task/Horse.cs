using System;

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

        public Horse()
        {
            BeingId = Guid.NewGuid().ToString();
        }

        public void EatHay()
        {
            Console.WriteLine("О. Опять сено. Люблю есть сено.");
        }
    }
}
