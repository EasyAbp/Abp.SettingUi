using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Authorization;
using EasyAbp.Abp.SettingUi.Dto;
using EasyAbp.Abp.SettingUi.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Xunit;
using P = EasyAbp.Abp.SettingUi.Authorization.TestSettingUiPermissionDefinitionProvider;

namespace EasyAbp.Abp.SettingUi.SettingUi
{
    /// <summary>
    /// Setting permissions named by the version 2.10 rule (the name only ends with the setting name) keep protecting
    /// their settings, fail closed, until the application renames them.
    /// </summary>
    public class SettingUiLegacySettingPermission_Tests : SettingUiApplicationTestBase
    {
        private const string OldSetting = "Old.Setting";
        private const string BothSetting = "Both.Setting";
        private const string TwinSetting = "Twin.Setting";

        private readonly ISettingUiAppService _service;
        private readonly FakePermissionChecker _permissionChecker = new();
        private readonly CapturingLoggerProvider _loggerProvider = new();

        public SettingUiLegacySettingPermission_Tests()
        {
            _service = GetRequiredService<ISettingUiAppService>();

            _permissionChecker.GrantedPermissions.Add(SettingUiPermissions.ShowSettingPage);
        }

        protected override void AfterAddApplication(IServiceCollection services)
        {
            services.AddSingleton<IPermissionChecker>(_permissionChecker);
            services.AddSingleton<ILoggerProvider>(_loggerProvider);

            var settingDefinitionManager = Substitute.For<ISettingDefinitionManager>();
            var settings = new[] { OldSetting, BothSetting, TwinSetting }
                .Select(name => new SettingDefinition(name)
                    .WithProperty(SettingUiConst.Group1, "Legacy")
                    .WithProperty(SettingUiConst.Group2, "Old"))
                .ToList();
            settingDefinitionManager.GetAllAsync().Returns(settings);
            foreach (var setting in settings)
            {
                settingDefinitionManager.GetOrNullAsync(setting.Name).Returns(setting);
            }

            services.AddSingleton(settingDefinitionManager);
            services.AddSingleton(Substitute.For<ISettingManager>());
            services.AddSingleton(Substitute.For<ISettingProvider>());
        }

        private async Task<List<SettingInfo>> GetVisibleSettingInfosAsync()
        {
            return (await _service.GroupSettingDefinitionsAsync()).SelectMany(group => group.SettingInfos).ToList();
        }

        private void Grant(params string[] permissionNames)
        {
            foreach (var permissionName in permissionNames)
            {
                _permissionChecker.GrantedPermissions.Add(permissionName);
            }
        }

        [Fact]
        public async Task A_Legacy_Named_Permission_Should_Still_Hide_The_Setting_From_A_User_Without_It()
        {
            (await GetVisibleSettingInfosAsync()).ShouldNotContain(si => si.Name == OldSetting);
        }

        [Fact]
        public async Task A_Legacy_Named_Permission_Should_Show_The_Setting_To_A_User_With_It_And_Log_A_Warning()
        {
            Grant(P.LegacyOldSetting);

            var setting = (await GetVisibleSettingInfosAsync()).Single(si => si.Name == OldSetting);

            setting.Permission.ShouldBe(P.LegacyOldSetting);
            _loggerProvider.Entries.ShouldContain(entry => entry.Level == LogLevel.Warning &&
                                                           entry.Message.Contains(P.LegacyOldSetting) &&
                                                           entry.Message.Contains("SettingUi.Legacy.Old.Old.Setting"));
        }

        [Fact]
        public async Task The_Exactly_Named_Permission_Should_Win_Over_A_Legacy_Named_One()
        {
            Grant(P.LegacyBothSetting);

            (await GetVisibleSettingInfosAsync()).ShouldNotContain(si => si.Name == BothSetting);

            _permissionChecker.GrantedPermissions.Remove(P.LegacyBothSetting);
            Grant(P.ExactBothSetting);

            (await GetVisibleSettingInfosAsync()).Single(si => si.Name == BothSetting).Permission
                .ShouldBe(P.ExactBothSetting);
            _loggerProvider.Entries.ShouldNotContain(entry => entry.Message.Contains(P.LegacyBothSetting));
        }

        [Fact]
        public async Task Several_Legacy_Named_Permissions_Should_All_Be_Required()
        {
            Grant(P.LegacyTwinSetting);

            (await GetVisibleSettingInfosAsync()).ShouldNotContain(si => si.Name == TwinSetting);

            _permissionChecker.GrantedPermissions.Remove(P.LegacyTwinSetting);
            Grant(P.OtherTwinSetting);

            (await GetVisibleSettingInfosAsync()).ShouldNotContain(si => si.Name == TwinSetting);

            Grant(P.LegacyTwinSetting);

            (await GetVisibleSettingInfosAsync()).ShouldContain(si => si.Name == TwinSetting);
        }
    }
}
