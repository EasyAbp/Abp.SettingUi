using EasyAbp.Abp.SettingUi;
using Volo.Abp.Account;
using Volo.Abp.Mapperly;
using Volo.Abp.Emailing;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.SettingManagement;

namespace MyAbpApp
{
    [DependsOn(
        typeof(MyAbpAppDomainModule),
        typeof(AbpAccountApplicationModule),
        typeof(MyAbpAppApplicationContractsModule),
        typeof(AbpMapperlyModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule),
        typeof(AbpSettingUiApplicationModule),
        typeof(AbpEmailingModule)
        )]
    [DependsOn(typeof(AbpSettingManagementApplicationModule))]
    public class MyAbpAppApplicationModule : AbpModule
    {
    }
}
