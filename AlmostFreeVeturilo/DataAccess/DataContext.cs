using AlmostFreeVeturilo.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace AlmostFreeVeturilo.DataAccess
{
    public class DataContext : DbContext
    {
        public DbSet<Station> Stations { get; set; }
        public DbSet<ConnectionOld> Connections { get; set; }

        public DataContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost; Database=AfvTest; Trusted_Connection=True;");
        }
    }
}