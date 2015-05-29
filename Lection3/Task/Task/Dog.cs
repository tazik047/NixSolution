using System;

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

        public Dog()
        {
            BeingId = Guid.NewGuid().ToString();
        }

        public void Breathe()
        {
            Console.WriteLine("Я задыхаюсь, не могу дышать под водой.");
        }
    }
}
