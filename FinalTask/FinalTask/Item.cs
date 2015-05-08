using System;
using System.IO;

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
            get { return string.Format("{0:F} MB",Size/(1024.0*1024));}
        }

        public void FillItem(FileSystemInfo fsi)
        {
            CreationDate = fsi.CreationTime;
            Name = fsi.Name;
        }
    }
}
