using EasyAbp.Abp.SettingUi.Web.Pages;
using EasyAbp.Abp.SettingUi.Web.Settings;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;
using Volo.Abp.VirtualFileSystem;

namespace EasyAbp.Abp.SettingUi.Web
{
    [DependsOn(
        typeof(AbpAspNetCoreMvcUiThemeSharedModule),
        typeof(AbpSettingManagementWebModule),
        typeof(AbpSettingUiApplicationContractsModule)
        )]
    public class AbpSettingUiWebModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpSettingUiWebModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpSettingUiWebModule>();
            });

            Configure<SettingManagementPageOptions>(options =>
            {
                options.Contributors.Add(new SettingUiSettingPageContributor());
            });

            Configure<AbpBundlingOptions>(options =>
            {
                options.ScriptBundles.Configure(typeof(Volo.Abp.SettingManagement.Web.Pages.SettingManagement.IndexModel).FullName, bundle =>
                    {
                        bundle.AddFiles("/client-proxies/settingUi-proxy.js");
                        bundle.AddFiles("/Pages/SettingManagement/SettingUi/Index.js");
                    }
                );
            });

            Configure<DynamicJavaScriptProxyOptions>(options =>
            {
                options.DisableModule(SettingUiRemoteServiceConsts.ModuleName);
            });
        }
    }
}
