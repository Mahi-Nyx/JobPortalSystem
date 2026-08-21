using System;
using System.ComponentModel.DataAnnotations;

namespace JobPortalSystem.Models
{
    public class User
    {
        [Key] // This tells ASP.NET this is the Primary Key (UserID)
        public int UserID { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Admin, Employer, or JobSeeker

        public string Status { get; set; } = "Pending";

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}