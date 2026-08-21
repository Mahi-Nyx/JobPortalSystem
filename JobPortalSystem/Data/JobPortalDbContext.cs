using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using JobPortalSystem.Models;

namespace JobPortalSystem.Data
{
    // Inheriting from DbContext makes this class our database bridge
    public class JobPortalDbContext : DbContext
    {
        // "DefaultConnection" tells EF to look for this name in our Web.config file
        public JobPortalDbContext() : base("name=JobPortalDBConnection")
        {
        }

        // This maps your C# User model to your SQL Server Users table
        public DbSet<User> Users { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Application> Applications { get; set; }
     
    }
}