using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.Abp.SettingUi.EntityFrameworkCore
{
    [ConnectionStringName(SettingUiDbProperties.ConnectionStringName)]
    public class SettingUiDbContext : AbpDbContext<SettingUiDbContext>, ISettingUiDbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * public DbSet<Question> Questions { get; set; }
         */

        public SettingUiDbContext(DbContextOptions<SettingUiDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureSettingUi();
        }
    }
}