using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobPortalSystem.Models
{
    [Table("Jobs")]
    public class Job
    {
        [Key]
        public int JobID { get; set; }

        public int UserID { get; set; }

        public string JobTitle { get; set; }

        public string CompanyName { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public string Salary { get; set; }

        public DateTime? Deadline { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}