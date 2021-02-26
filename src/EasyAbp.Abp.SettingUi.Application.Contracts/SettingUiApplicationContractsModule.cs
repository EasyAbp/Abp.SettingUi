using System.Collections.Generic;
using EasyAbp.Abp.SettingUi.Dto;
using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.Authorization;
using Volo.Abp.Json.SystemTextJson;

namespace EasyAbp.Abp.SettingUi
{
    [DependsOn(
        typeof(SettingUiDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule)
        )]
    public class SettingUiApplicationContractsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpSystemTextJsonSerializerOptions>(options =>
            {
                // System.Text.Json seems cannot deserialize the Dictionary<string, object> type properly,
                // So we let JSON.NET do this
                options.UnsupportedTypes.AddIfNotContains(typeof(List<SettingGroup>));
            });
        }
    }
}
