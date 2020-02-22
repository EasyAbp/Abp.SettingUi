using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace EasyAbp.Abp.SettingUi.EntityFrameworkCore
{
    public class SettingUiModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public SettingUiModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = "",
            [CanBeNull] string schema = null)
            : base(
                tablePrefix,
                schema)
        {

        }
    }
}