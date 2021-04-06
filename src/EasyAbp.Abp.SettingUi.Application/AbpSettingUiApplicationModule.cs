using Volo.Abp.Modularity;
using Volo.Abp.Application;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(AbpSettingUiDomainModule),
        typeof(AbpSettingUiApplicationContractsModule),
        typeof(AbpDddApplicationModule)
        )]
    public class AbpSettingUiApplicationModule : AbpModule
    {
    }
}
