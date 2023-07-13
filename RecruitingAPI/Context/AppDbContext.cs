using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecruitingAPI.Models;
using System.Reflection;

namespace RecruitingAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Company> Companies { get; set; }

        public DbSet<Candidate> Candidates { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<Recruiter> Recruiters { get; set; }

        public DbSet<Offer> Offers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var splitStringConverter = new ValueConverter<string[], string>(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries));

            var splitStringValueComparer = new ValueComparer<string[]>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToArray());

            modelBuilder.Entity<Offer>()
                .Property(e => e.skills)
                .HasConversion(splitStringConverter, splitStringValueComparer);

            modelBuilder.Entity<Offer>()
                .Property(e => e.missions)
                .HasConversion(splitStringConverter, splitStringValueComparer);

            modelBuilder.Entity<Offer>()
                .Property(e => e.languages)
                .HasConversion(splitStringConverter, splitStringValueComparer);
        }

        public DbSet<Candidature> Candidatures { get; set; }

        public DbSet<Visitor> Visitors { get; set; }

        public DbSet<Interview> Interviews { get; set; }

        public DbSet<Contact> Contact { get; set; }



    }
}
