using System;
using System.Collections.Generic;
using System.Text;

namespace ArcConv.Models
{
    public enum FileListStatus
    {
        NOT_PROCESSED,
        IN_PROCESS,
        PROCESSED,
        ERROR,
    }

    public class FileListItem
    {
        public string FileName { set; get; }
        public string FilePath { set; get; }
        public FileListStatus Status { set; get; }
    }
}
