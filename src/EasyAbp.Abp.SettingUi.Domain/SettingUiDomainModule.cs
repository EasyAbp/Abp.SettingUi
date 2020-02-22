using Volo.Abp.Modularity;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(SettingUiDomainSharedModule)
        )]
    public class SettingUiDomainModule : AbpModule
    {

    }
}
