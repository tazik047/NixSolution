using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinalTask
{
    class ConcurrentQueue<T> where T : class
    {
        private Queue<T> _queue = new Queue<T>();

        private ReaderWriterLock _lock = new ReaderWriterLock();

        public bool IsEmpty { get { return _queue.Count == 0; }}

        public void Enqueue(T item)
        {
            _lock.AcquireWriterLock(Timeout.Infinite);
            _queue.Enqueue(item);
            _lock.ReleaseWriterLock();
        }

        public T Dequeue()
        {
            if (IsEmpty) return null;
            _lock.AcquireWriterLock(Timeout.Infinite);
            var item = _queue.Dequeue();
            _lock.ReleaseWriterLock();
            return item;
        }
    }
}
