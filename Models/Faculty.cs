using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolProject1640.Models
{
    public class Faculty
    {
        [Key]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
