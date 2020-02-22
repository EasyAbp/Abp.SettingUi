using System;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace EasyAbp.Abp.SettingUi.MongoDB
{
    public static class SettingUiMongoDbContextExtensions
    {
        public static void ConfigureSettingUi(
            this IMongoModelBuilder builder,
            Action<AbpMongoModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new SettingUiMongoModelBuilderConfigurationOptions(
                SettingUiDbProperties.DbTablePrefix
            );

            optionsAction?.Invoke(options);
        }
    }
}