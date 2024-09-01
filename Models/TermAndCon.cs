using System.ComponentModel.DataAnnotations;

namespace SchoolProject1640.Models
{
    public class TermAndCon
    {
        [Key] public string Id { get; set; }
        public string? TermsAndCondition { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
