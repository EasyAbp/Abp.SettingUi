using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EasyAbp.Abp.SettingUi.EntityFrameworkCore
{
    public class SettingUiHttpApiHostMigrationsDbContextFactory : IDesignTimeDbContextFactory<SettingUiHttpApiHostMigrationsDbContext>
    {
        public SettingUiHttpApiHostMigrationsDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<SettingUiHttpApiHostMigrationsDbContext>()
                .UseSqlServer(configuration.GetConnectionString("SettingUi"));

            return new SettingUiHttpApiHostMigrationsDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
