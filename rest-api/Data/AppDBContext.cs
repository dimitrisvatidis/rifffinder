using Microsoft.EntityFrameworkCore;
using rifffinder.Models;
using System.Collections.Generic;

namespace rifffinder.Data 
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Musician> Musicians { get; set; }
        public DbSet<Band> Bands { get; set; }
        public DbSet<Posting> Postings { get; set; }

        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Request>()
                .Property(r => r.Status)
                .HasConversion<string>(); 

            modelBuilder.Entity<Posting>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Posting>()
                .HasOne(p => p.Band)
                .WithMany()
                .HasForeignKey(p => p.BandId);
        }
    }
}
