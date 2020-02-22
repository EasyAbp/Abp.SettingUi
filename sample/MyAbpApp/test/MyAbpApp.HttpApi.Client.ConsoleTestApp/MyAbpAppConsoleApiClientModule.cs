using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace MyAbpApp.HttpApi.Client.ConsoleTestApp
{
    [DependsOn(
        typeof(MyAbpAppHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class MyAbpAppConsoleApiClientModule : AbpModule
    {
        
    }
}
