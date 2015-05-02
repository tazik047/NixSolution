using System;
using System.IO;

namespace FinalTask
{
    public class Item
    {
        public string Name { get; set; }

        public string Path { get; set; }
        
        public DateTime CreationDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public DateTime LastAccessDate { get; set; }

        public FileAttributes Attributes { get; set; }

        public ItemType Type { get; set; }

        public string Owner { get; set; }

        public string Rights { get; set; }

        public long Size { get; set; }

        public void FillItem(FileSystemInfo fsi)
        {
            Attributes = fsi.Attributes;
            CreationDate = fsi.CreationTime;
            LastAccessDate = fsi.LastAccessTime;
            LastModifiedDate = fsi.LastWriteTime;
            Name = fsi.Name;
            Path = fsi.FullName;
            Rights = "";
        }
    }
}
