using JetBrains.Annotations;
using Volo.Abp.MongoDB;

namespace EasyAbp.Abp.SettingUi.MongoDB
{
    public class SettingUiMongoModelBuilderConfigurationOptions : AbpMongoModelBuilderConfigurationOptions
    {
        public SettingUiMongoModelBuilderConfigurationOptions(
            [NotNull] string collectionPrefix = "")
            : base(collectionPrefix)
        {
        }
    }
}