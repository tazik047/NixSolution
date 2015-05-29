namespace Task
{
    /// <summary>
    /// Животное
    /// </summary>
    abstract class Animal : Being
    {
        /// <summary>
        /// Свойство только для чтения, которое возвращает количество
        /// ног конкретного существа.
        /// </summary>
        public abstract int CountLegs { get; }
    }
}
