using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.Abp.SettingUi.EntityFrameworkCore
{
    public class SettingUiHttpApiHostMigrationsDbContext : AbpDbContext<SettingUiHttpApiHostMigrationsDbContext>
    {
        public SettingUiHttpApiHostMigrationsDbContext(DbContextOptions<SettingUiHttpApiHostMigrationsDbContext> options)
            : base(options)
        {
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigureSettingUi();
        }
    }
}
