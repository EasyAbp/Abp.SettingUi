using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.Abp.SettingUi.EntityFrameworkCore
{
    [ConnectionStringName(SettingUiDbProperties.ConnectionStringName)]
    public interface ISettingUiDbContext : IEfCoreDbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * DbSet<Question> Questions { get; }
         */
    }
}