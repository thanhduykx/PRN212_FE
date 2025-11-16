using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Context
{
    public class DbContextFactory : IDesignTimeDbContextFactory<EVSDbContext>
    {
        public EVSDbContext CreateDbContext(string[] args)
        {

            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "EVStation-basedRentalSystem");
            var config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<EVSDbContext>();
            var connectionString = config.GetConnectionString("Database");

            optionsBuilder.UseSqlServer(connectionString);

            return new EVSDbContext(optionsBuilder.Options);
        }
    }
}
