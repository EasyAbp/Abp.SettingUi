using Volo.Abp.Modularity;

namespace MyAbpApp
{
    [DependsOn(
        typeof(MyAbpAppApplicationModule),
        typeof(MyAbpAppDomainTestModule)
        )]
    public class MyAbpAppApplicationTestModule : AbpModule
    {

    }
}