using System;

namespace TaskD
{
    /* Класс должен реализовывать интерфейс IComparable<MyGeneric<T>> для того,
     * чтобы экземпляры данного класса можно было сравнивать.
     * На параметр Т накладываются такие ограничения: он должен быть структурой
     * (значимым типом) и соотвественно из-за этого он не может быть равен null,
     * и так же этот тип должен тоже реализовывать интерфейм IComparable<T>, чтобы
     * иметь возможность сравнивать эти объекты.
     */
    class MyGeneric<T> : IComparable<MyGeneric<T>> where T : struct, IComparable<T>
    {
        public T Property { get; set; }

        public MyGeneric() : this(default(T)) { }

        public MyGeneric(T element)
        {
            Property = element;
        }

        /* Необходимо переопределить ToString, чтоб при распечатке
         * данного объекта выводилась нужная нам информация.
         */
        public override string ToString()
        {
            return Property.ToString();
        }

        public int CompareTo(MyGeneric<T> other)
        {
            return Property.CompareTo(other.Property);
        }
    }
}
