using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.Threading;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAuthorizationModule), 
        typeof(AbpSettingUiDomainModule),
        typeof(AbpSettingManagementDomainModule)
    )]
    public class SettingUiTestBaseModule : AbpModule
    {
    }
}