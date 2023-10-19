using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAuthorizationModule), 
        typeof(AbpSettingUiDomainModule),
        typeof(AbpSettingManagementDomainModule),
        typeof(AbpSettingManagementEntityFrameworkCoreModule)
    )]
    public class SettingUiTestBaseModule : AbpModule
    {
    }
}