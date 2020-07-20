using EasyAbp.Abp.SettingUi.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using Volo.Abp.SettingManagement;
using Volo.Abp.UI;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(AbpValidationModule),
        typeof(AbpUiModule),
        typeof(AbpSettingManagementDomainSharedModule)
    )]
    public class SettingUiDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options => { options.FileSets.AddEmbedded<SettingUiDomainSharedModule>(); });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<SettingUiResource>("en")
                    .AddVirtualJson("/Localization/SettingUi");
            });
        }
    }
}