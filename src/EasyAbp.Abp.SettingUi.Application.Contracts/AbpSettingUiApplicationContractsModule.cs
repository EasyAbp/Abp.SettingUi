using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(AbpSettingUiDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule)
        )]
    public class AbpSettingUiApplicationContractsModule : AbpModule
    {
    }
}
