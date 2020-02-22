using Volo.Abp.Modularity;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(SettingUiApplicationModule),
        typeof(SettingUiDomainTestModule)
        )]
    public class SettingUiApplicationTestModule : AbpModule
    {

    }
}
