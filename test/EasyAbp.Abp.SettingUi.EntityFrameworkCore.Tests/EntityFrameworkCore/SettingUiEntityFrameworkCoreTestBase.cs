using Volo.Abp.Modularity;

namespace EasyAbp.Abp.SettingUi.EntityFrameworkCore
{
    /* This class can be used as a base class for EF Core integration tests,
     * while SampleRepository_Tests uses a different approach.
     */
    [DependsOn(
        typeof(SettingUiTestBaseModule)
    )]  public abstract class SettingUiEntityFrameworkCoreTestBase : SettingUiTestBase<SettingUiEntityFrameworkCoreTestModule>
    {

    }
}