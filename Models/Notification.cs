
using System.ComponentModel.DataAnnotations;

namespace SchoolProject1640.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string? UserID { get; set; }
        public string? Message { get; set; }
        public string? FacultyId { get; set; }
        public string? RoleId { get; set; }
        public string? SendBy { get; set; }
        public bool? isRead { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;

    }
}
