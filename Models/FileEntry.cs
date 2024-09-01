using System.ComponentModel.DataAnnotations;

namespace SchoolProject1640.Models
{
    public class FileEntry
    {
        [Key]
        public int Id { get; set; }
        public int? Idtest {  get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
    }
}
