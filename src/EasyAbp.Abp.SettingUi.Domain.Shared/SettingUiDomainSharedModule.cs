using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using EasyAbp.Abp.SettingUi.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.SettingManagement;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(AbpValidationModule),
        typeof(AbpSettingManagementDomainSharedModule)
    )]
    public class SettingUiDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<SettingUiDomainSharedModule>("EasyAbp.Abp.SettingUi");
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<SettingUiResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/Localization/SettingUi");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("SettingUi", typeof(SettingUiResource));
            });
        }
    }
}
