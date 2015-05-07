using System;
using System.IO;
using System.Xml.Linq;

namespace FinalTask
{
    public class Item
    {
        public string Name { get; set; }
        
        public DateTime CreationDate { get; set; }

        public string Created
        {
            get { return CreationDate.ToString("G"); }
        }

        public ItemType Type { get; set; }

        public long Size { get; set; }

        public string SizeMb
        {
            get {
                if (Size > 1024l * 1024 * 1024)
                {
                    return string.Format("{0:F} GB",Size/(1024.0 * 1024 * 1024));
                }
                if (Size > 1024l * 1024)
                {
                    return string.Format("{0:F} MB",Size/(1024.0 * 1024));
                }
                if (Size > 1024)
                {
                    return string.Format("{0:F} KB", Size / 1024.0);
                }
                return string.Format("{0} B", Size);
            }
        }

        public void FillItem(FileSystemInfo fsi)
        {
            CreationDate = fsi.CreationTime;
            Name = fsi.Name;
        }
    }
}
