using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(AbpSettingUiDomainSharedModule),
        typeof(AbpSettingManagementDomainModule)
        )]
    public class AbpSettingUiDomainModule : AbpModule
    {

    }
}
