using Microsoft.Extensions.DependencyInjection;
using EasyAbp.Abp.SettingUi.Web.Pages;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;
using Volo.Abp.VirtualFileSystem;

namespace EasyAbp.Abp.SettingUi.Web
{
    [DependsOn(
        typeof(AbpAspNetCoreMvcUiThemeSharedModule),
        typeof(AbpSettingManagementWebModule)
        )]
    [DependsOn(typeof(SettingUiHttpApiModule))]
    public class SettingUiWebModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(SettingUiWebModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<SettingUiWebModule>("EasyAbp.Abp.SettingUi.Web");
            });

            Configure<SettingManagementPageOptions>(options =>
            {
                options.Contributors.Add(new SettingUiPageContributor());
            });
        }
    }
}
