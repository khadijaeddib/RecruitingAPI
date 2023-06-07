using Microsoft.EntityFrameworkCore;
using RecruitingAPI.Models;

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



        /* protected override void OnModelCreating(ModelBuilder modelBuilder)
         {
             modelBuilder.Entity<Candidate>().ToTable("Candidates");
         }*/
    }
}
