using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace EasyAbp.Abp.SettingUi.MongoDB
{
    [ConnectionStringName(SettingUiDbProperties.ConnectionStringName)]
    public class SettingUiMongoDbContext : AbpMongoDbContext, ISettingUiMongoDbContext
    {
        /* Add mongo collections here. Example:
         * public IMongoCollection<Question> Questions => Collection<Question>();
         */

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);

            modelBuilder.ConfigureSettingUi();
        }
    }
}