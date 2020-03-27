using System;
using System.Collections.Generic;
using System.Text;

namespace NCovid.Service.DataContext
{
    using Microsoft.EntityFrameworkCore;

   public class CoronaDbContext: DbContext
    {
        public DbSet<Countries> Countries { get; set; }
        public DbSet<GlobalInfo> All { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=corona.db");
    }
}
