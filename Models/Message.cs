using System.ComponentModel.DataAnnotations;

namespace SchoolProject1640.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string? AccountID { get; set; }
        public int? ArtID { get; set; }
        public string? Mess { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
