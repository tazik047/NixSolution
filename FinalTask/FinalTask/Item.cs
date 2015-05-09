using System;
using System.IO;

namespace FinalTask
{
    public class Item
    {
        /// <summary>
        /// Имя папки или файла.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Дата создания в общем шаблоне даты и времени (полный шаблон времени).
        /// </summary>
        public string Created
        {
            get { return CreationDate.ToString("G"); }
        }

        /// <summary>
        /// Тип элемента
        /// </summary>
        public ItemType Type { get; set; }

        /// <summary>
        /// Размер в байтах.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Форматированный размер
        /// </summary>
        public string SizeMb
        {
            get {
                if (Size > 1024L * 1024 * 1024) // если размер больше 1 Гб
                {
                    return string.Format("{0:F} GB",Size/(1024.0 * 1024 * 1024));
                }
                if (Size > 1024L * 1024) // если размер больше 1 Мб
                {
                    return string.Format("{0:F} MB",Size/(1024.0 * 1024));
                }
                if (Size > 1024) // если размер больше 1 Кб
                {
                    return string.Format("{0:F} KB", Size / 1024.0);
                }
                return string.Format("{0} B", Size);
            }
        }

        /// <summary>
        /// Вспомогательный метод для заполнения элемента из элемента файловой системы.
        /// </summary>
        /// <param name="fsi">Исходные данные</param>
        public void FillItem(FileSystemInfo fsi)
        {
            CreationDate = fsi.CreationTime;
            Name = fsi.Name;
        }
    }
}