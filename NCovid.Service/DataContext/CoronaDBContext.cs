using System;
using System.Collections.Generic;
using System.Text;

namespace NCovid.Service.DataContext
{
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore;

    public class CoronaDbContext: DbContext
    {
        public CoronaDbContext(DbContextOptions options) : base(options)
        {
        }


        public DbSet<Countries> Countries { get; set; }
        public DbSet<GlobalInfo> All { get; set; }
        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
        //    base.OnConfiguring(options);
        //}
    }
}


