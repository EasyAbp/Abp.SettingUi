using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace EasyAbp.Abp.SettingUi.MongoDB
{
    [ConnectionStringName(SettingUiDbProperties.ConnectionStringName)]
    public interface ISettingUiMongoDbContext : IAbpMongoDbContext
    {
        /* Define mongo collections here. Example:
         * IMongoCollection<Question> Questions { get; }
         */
    }
}
