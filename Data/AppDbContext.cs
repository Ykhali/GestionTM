//using GestionTM.Models;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using System.ComponentModel.DataAnnotations;
//using System.Reflection.Metadata;

//namespace GestionTM.Data
//{
//    public class AppDbContext : IdentityDbContext
//    {
//        public DbSet<Admin> Admins { get; set; }
//        public DbSet<Consultant> Consultants { get; set; }
//        public DbSet<Validateur> Validateurs { get; set; }
//        public DbSet<Timesheet> Timesheets { get; set; }
//        public DbSet<TimesheetLine> TimesheetLines { get; set; }

//        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//            modelBuilder.Entity<Consultant>()
//                .HasOne(c => c.Validateur)
//                .WithMany(v => v.Consultants)
//                .HasForeignKey(c => c.ValidateurId)
//                .OnDelete(DeleteBehavior.SetNull);
//        }
//    }
//}

using GestionTM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace GestionTM.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Consultant> Consultants { get; set; }
        public DbSet<Validateur> Validateurs { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<TimesheetLine> TimesheetLines { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Consultant>()
                .HasOne(c => c.Validateur)
                .WithMany(v => v.Consultants)
                .HasForeignKey(c => c.ValidateurId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

