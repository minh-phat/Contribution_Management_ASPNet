using System.ComponentModel.DataAnnotations;

namespace SchoolProject1640.Models
{
    public class Submission
    {
        [Key]
        public int Id { get; set; }
        public int? ContributionId { get; set; } // Submission
        public string? AccountId { get; set; } // ten nguoi dung
        public int? State { get; set; } // 1 pending // 2 complete 3 4 ...
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
