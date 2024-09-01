using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SchoolProject1640.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add any additional properties or methods here
        public string? FacultyId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Image { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastModifiedDate { get; set; }
        public ApplicationUser? UpdateBy { get; set; }
    }
}