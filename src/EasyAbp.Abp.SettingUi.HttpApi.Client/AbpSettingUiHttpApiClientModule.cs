using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(AbpSettingUiApplicationContractsModule),
        typeof(AbpHttpClientModule))]
    public class AbpSettingUiHttpApiClientModule : AbpModule
    {
        public const string RemoteServiceName = "EasyAbpAbpSettingUi";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(AbpSettingUiApplicationContractsModule).Assembly,
                RemoteServiceName
            );
        }
    }
}
