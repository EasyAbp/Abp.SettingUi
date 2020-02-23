using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace EasyAbp.Abp.SettingUi
{
    /* Domain tests are configured to use the EF Core provider.
     * You can switch to MongoDB, however your domain tests should be
     * database independent anyway.
     */
    [DependsOn(
        typeof(SettingUiTestBaseModule),
        typeof(SettingUiDomainModule),
        typeof(AbpEntityFrameworkCoreModule)
    )]
    public class SettingUiDomainTestModule : AbpModule
    {
        
    }
}
