using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(SettingUiDomainSharedModule),
        typeof(AbpSettingManagementDomainModule)
        )]
    public class SettingUiDomainModule : AbpModule
    {

    }
}
