using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(SettingUiApplicationModule),
        typeof(SettingUiDomainTestModule)
        )]
    public class SettingUiApplicationTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<SettingUiApplicationTestModule>("EasyAbp.Abp.SettingUi");
            });
        }
    }
}
