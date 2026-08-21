using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobPortalSystem.Models
{
    [Table("Applications")]
    public class Application
    {
        [Key]
        public int ApplicationID { get; set; }

        [Required]
        public int JobID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        [StringLength(200)]
        public string Education { get; set; }

        [Required]
        [StringLength(500)]
        public string CVPath { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        public DateTime AppliedDate { get; set; }

        [ForeignKey("JobID")]
        public virtual Job Job { get; set; }
    }
}