using Localization.Resources.AbpUi;
using EasyAbp.Abp.SettingUi.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(AbpSettingUiApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule))]
    public class AbpSettingUiHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpSettingUiHttpApiModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<SettingUiResource>()
                    .AddBaseTypes(typeof(AbpUiResource));
            });
        }
    }
}
