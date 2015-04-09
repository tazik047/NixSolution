using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskB
{
    internal class MyGenericList<T> : IEnumerable<T>, ICollection<T>
    {

        private List<T> _list;
        private const int _maxListCapacity = 5;
        private SortedList<T, T> _sortedList;

        public MyGenericList()
        {
            _list = new List<T>();
        }

        public MyGenericList(IEnumerable<T> items)
            : this()
        {
            foreach (T item in items)
                Add(item);
        }

        public T this[int index]
        {
            get
            {
                // Если индекс будет за границами массива, 
                //то будет сгенерировано исключение: ArgumentOutOfRangeException
                return _sortedList == null ? _list[index] : _sortedList.Keys[index];
            }
        }

        public int Count
        {
            get { return _sortedList == null ? _list.Count : _sortedList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(T item)
        {
            if (_sortedList == null)
            {
                //Запрещаем в список добавлять одинаковые элементы.
                if (_list.Contains(item))
                    throw new ArgumentException("Запись с таким значением уже существует");
                if (_list.Count == _maxListCapacity)
                {
                    _sortedList = new SortedList<T, T>(_list.Count + 1);
                    foreach (T i in _list)
                    {
                        _sortedList.Add(i, i);
                    }
                    _list = null;
                    Add(item);
                    return;
                }
                _list.Add(item);
            }
            else
            {
                //При добавлении записи с тем же значением будет сгенерировано исключение ArgumentException
                _sortedList.Add(item, item);
            }
        }

        public void Clear()
        {
            if (_sortedList == null)
                _list.Clear();
            else
            {
                _sortedList = null;
                _list = new List<T>();
            }
        }

        public bool Contains(T item)
        {
            if (_sortedList == null)
                return _list.Contains(item);
            return _sortedList.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (_sortedList == null)
                _list.CopyTo(array, arrayIndex);
            else
            {
                //Проверяем поместяться ли все элементы списка в новый массив.
                if (array.Length - arrayIndex < _sortedList.Count)
                    throw new ArgumentException("Длина результирующего массива недостаточна.");
                foreach (var i in _sortedList)
                {
                    array[arrayIndex++] = i.Key;
                }
            }
        }

        public bool Remove(T item)
        {
            if (_sortedList == null)
            {
                return _list.Remove(item);
            }
            else
            {
                var res = _sortedList.Remove(item);
                // Если элементов станет 5, то опять переключиться на список.
                if (_sortedList.Count == _maxListCapacity)
                {
                    // Копируем все значения обратно в список.
                    _list = _sortedList.Select(e => e.Value).ToList();
                    _sortedList = null;
                }
                return res;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Реализация IEnumerator<T> для перебора элементов в foreach.
        /// </summary>
        private class Enumerator : IEnumerator<T>
        {
            private readonly MyGenericList<T> _list;
            private int index;
            public T Current
            {
                get { return _list[index]; }
            }

            public Enumerator(MyGenericList<T> list)
            {
                _list = list;
                //Индекс должен указывать на элемент идущий перед начальным.
                index = -1; 
            }

            public void Dispose()
            {
                index = -1;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (index == _list.Count - 1)
                {
                    Reset();
                    return false;
                }
                index++;
                return true;
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }
}
