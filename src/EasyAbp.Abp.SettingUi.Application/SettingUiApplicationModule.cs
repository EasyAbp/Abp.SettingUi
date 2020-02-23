using Volo.Abp.Modularity;
using Volo.Abp.Application;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(SettingUiDomainModule),
        typeof(SettingUiApplicationContractsModule),
        typeof(AbpDddApplicationModule)
        )]
    public class SettingUiApplicationModule : AbpModule
    {
    }
}
