using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AFVTry.Models;
using AFVTry.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace AFVTry.DataAccess
{
    public class DataContext : DbContext
    {
        public DbSet<Station> Stations { get; set; }
        public DbSet<Connection_old> Connections { get; set; }

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