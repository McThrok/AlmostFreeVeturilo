using AlmostFreeVeturilo.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace AlmostFreeVeturilo.DataAccess
{
    public class DataContext : DbContext
    {
        public DbSet<Connection> Connections { get; set; }

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