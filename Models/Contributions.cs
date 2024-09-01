using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolProject1640.Models
{
    public class Contribution
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Faculty { get; set; }
        public string? AcademicYear { get; set; }
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
        public DateTime ClosureDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
        public DateTime FinalClosureDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }
}
