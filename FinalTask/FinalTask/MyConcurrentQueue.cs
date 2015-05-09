using System.Collections.Generic;

namespace FinalTask
{
    /// <summary>
    /// Реализация очереди с поддержкой многопоточности.
    /// </summary>
    /// <typeparam name="T">Класс, с которым необходимо работать в очереди</typeparam>
    class MyConcurrentQueue<T> where T : class
    {
        private readonly Queue<T> _queue = new Queue<T>();

        private readonly object _lock = new object();

        /// <summary>
        /// Пустая ли очередь
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                int count;
                lock (_lock)
                    count = _queue.Count;
                return count == 0;
            }
        }

        /// <summary>
        /// Добавляет элемент в очередь.
        /// </summary>
        /// <param name="item">Элемент для добавления.</param>
        public void Enqueue(T item)
        {
            lock(_lock)
                _queue.Enqueue(item);
        }

        /// <summary>
        /// Получает элемент из очерери.
        /// </summary>
        /// <returns>Полученный элемент</returns>
        public T Dequeue()
        {
            if (IsEmpty) return null;
            T item;
            lock(_lock)
                item = _queue.Dequeue();
            return item;
        }
    }
}
