using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Xunit;

namespace EasyAbp.Abp.SettingUi.SettingUi
{
    public class SettingUiSettingPermission_Tests : SettingUiApplicationTestBase
    {
        private const string ServerIpSetting = "Server.Ip";
        private const string IpSetting = "Ip";

        private readonly ISettingUiAppService _service;
        private readonly FakePermissionChecker _permissionChecker = new();
        private ISettingManager _settingManager;

        public SettingUiSettingPermission_Tests()
        {
            _service = GetRequiredService<ISettingUiAppService>();

            _permissionChecker.GrantedPermissions.Add(SettingUiPermissions.ShowSettingPage);
            _permissionChecker.GrantedPermissions.Add(TestSettingUiPermissionDefinitionProvider.NetworkGroup);
            _permissionChecker.GrantedPermissions.Add(TestSettingUiPermissionDefinitionProvider.NetworkAddressGroup);
        }

        protected override void AfterAddApplication(IServiceCollection services)
        {
            services.AddSingleton<IPermissionChecker>(_permissionChecker);

            var settingDefinitionManager = Substitute.For<ISettingDefinitionManager>();
            var serverIpSetting = new SettingDefinition(ServerIpSetting)
                .WithProperty(SettingUiConst.Group1, "Network")
                .WithProperty(SettingUiConst.Group2, "Address");
            var ipSetting = new SettingDefinition(IpSetting)
                .WithProperty(SettingUiConst.Group1, "Network")
                .WithProperty(SettingUiConst.Group2, "Address");
            settingDefinitionManager.GetAllAsync().Returns(new List<SettingDefinition>
            {
                serverIpSetting,
                ipSetting
            });
            settingDefinitionManager.GetOrNullAsync(ServerIpSetting).Returns(serverIpSetting);
            settingDefinitionManager.GetOrNullAsync(IpSetting).Returns(ipSetting);
            services.AddSingleton(settingDefinitionManager);

            _settingManager = Substitute.For<ISettingManager>();
            services.AddSingleton(_settingManager);

            services.AddSingleton(Substitute.For<ISettingProvider>());
            // Read by the setting value providers, to tell where each value comes from.
            services.AddSingleton(Substitute.For<ISettingStore>());
        }

        private async Task<List<string>> GetVisibleSettingNamesAsync()
        {
            return (await _service.GroupSettingDefinitionsAsync())
                .SelectMany(group => group.SettingInfos)
                .Select(settingInfo => settingInfo.Name)
                .ToList();
        }

        [Fact]
        public async Task A_Setting_Should_Use_Only_The_Permission_Named_After_Its_Groups_And_Name()
        {
            var settingInfos = (await _service.GroupSettingDefinitionsAsync()).SelectMany(group => group.SettingInfos);

            // Nothing granted beyond the groups: both settings are hidden by their own permissions.
            settingInfos.ShouldBeEmpty();

            _permissionChecker.GrantedPermissions.Add(TestSettingUiPermissionDefinitionProvider.ServerIpSetting);
            _permissionChecker.GrantedPermissions.Add(TestSettingUiPermissionDefinitionProvider.IpSetting);

            settingInfos = (await _service.GroupSettingDefinitionsAsync()).SelectMany(group => group.SettingInfos).ToList();

            settingInfos.Single(si => si.Name == ServerIpSetting).Permission
                .ShouldBe(TestSettingUiPermissionDefinitionProvider.ServerIpSetting);
            settingInfos.Single(si => si.Name == IpSetting).Permission
                .ShouldBe(TestSettingUiPermissionDefinitionProvider.IpSetting);
        }

        [Fact]
        public async Task Another_Settings_Permission_Should_Not_Show_A_Setting()
        {
            // "Server.Ip" ends with "Ip", but its permission must not stand in for the "Ip" permission.
            _permissionChecker.GrantedPermissions.Add(TestSettingUiPermissionDefinitionProvider.ServerIpSetting);

            (await GetVisibleSettingNamesAsync()).ShouldBe(new[] { ServerIpSetting });

            await Should.ThrowAsync<AbpAuthorizationException>(() => _service.SetSettingValuesAsync(
                new Dictionary<string, string> { { "setting_Ip", "10.0.0.1" } }));
            await _settingManager.DidNotReceiveWithAnyArgs().SetAsync(default, default, default, default, default);
        }

        [Fact]
        public async Task Another_Settings_Permission_Should_Not_Hide_A_Setting()
        {
            // The "Ip" permission is granted; the ungranted "Server.Ip" permission must not hide the "Ip" setting.
            _permissionChecker.GrantedPermissions.Add(TestSettingUiPermissionDefinitionProvider.IpSetting);

            (await GetVisibleSettingNamesAsync()).ShouldBe(new[] { IpSetting });

            await _service.SetSettingValuesAsync(new Dictionary<string, string> { { "setting_Ip", "10.0.0.1" } });
            await _settingManager.Received(1).SetForCurrentTenantAsync(IpSetting, "10.0.0.1");
        }
    }
}
