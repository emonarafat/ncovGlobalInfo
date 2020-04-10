using System;
using System.Collections.Generic;
using System.Text;

namespace NCovid.Service.DataContext
{
    using System.IO;
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class CoronaDbContext: DbContext
    {
        public CoronaDbContext(DbContextOptions options) : base(options)
        {
        }
        private CoronaDbContext()
        {
        }
        public static CoronaDbContext GetContext() => new CoronaDbContext();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddJsonFile(
                   "appsettings.development.json",
                   reloadOnChange: true,
                   optional: true).Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("SqlConnection"));

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<CoronaInfo> CoronaInfos { get; set; }

        public DbSet<Countries> Countries { get; set; }

        public DbSet<GlobalInfo> All { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CoronaInfo>()
                .HasMany(c => c.Countries)
                .WithOne(e => e.CoronaInfo)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}


