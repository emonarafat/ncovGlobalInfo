namespace NCovid.Service.DataContext
{
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CoronaDbContext>
    {
        /// <summary>
        /// The CreateDbContext
        /// </summary>
        /// <param name="args">The args<see cref="string[]"/></param>
        /// <returns>The <see cref="CoronaDbContext"/></returns>
        public CoronaDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").AddJsonFile(
                    "appsettings.development.json",
                    reloadOnChange: true,
                    optional: true).Build();

            var builder = new DbContextOptionsBuilder<CoronaDbContext>();
            builder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            return new CoronaDbContext(builder.Options);
        }
    }
}