using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(SettingUiApplicationContractsModule),
        typeof(AbpHttpClientModule))]
    public class SettingUiHttpApiClientModule : AbpModule
    {
        public const string RemoteServiceName = "SettingUi";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(SettingUiApplicationContractsModule).Assembly,
                RemoteServiceName
            );
        }
    }
}
