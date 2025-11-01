using CollabHub.Domain.Commom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class Files:BaseEntity
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FileData { get; set; }
        public string FileExtension { get; set; }
        public decimal FileSizeInKB { get; set; }
    }
}
