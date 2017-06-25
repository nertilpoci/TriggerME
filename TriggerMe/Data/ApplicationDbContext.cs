using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using WebApplicationBasic.Models;
using TriggerMe.Model;
using Z.EntityFramework.Plus;

namespace WebApplicationBasic.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<TriggerMe.Model.Client> Client { get; set; }

        public DbSet<TriggerMe.Model.TriggerMessage> TriggerMessage { get; set; }
        public DbSet<TriggerMe.Model.Connection> Connections { get; set; }
    }
}
