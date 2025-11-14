using CollabHub.Domain.Commom;
using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class FileResource:BaseEntity
    {
        [Key]
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string? FileData { get; set; }

        public string? FilePath { get; set; }
        public string FileExtension { get; set; }
        public decimal FileSizeInKB { get; set; }
        public FileContextType ContextType { get; set; }
        public int? ReferenceId { get; set; }
        public User ReferenceUser { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
