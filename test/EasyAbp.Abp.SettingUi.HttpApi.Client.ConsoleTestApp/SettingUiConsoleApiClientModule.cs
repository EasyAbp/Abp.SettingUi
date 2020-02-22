using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(SettingUiHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class SettingUiConsoleApiClientModule : AbpModule
    {
        
    }
}
