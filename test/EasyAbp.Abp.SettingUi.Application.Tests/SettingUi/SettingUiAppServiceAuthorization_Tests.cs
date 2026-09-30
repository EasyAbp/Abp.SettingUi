using System.Collections.Generic;
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
    public class SettingUiAppServiceAuthorization_Tests : SettingUiApplicationTestBase
    {
        private const string PublicSetting = "Test.Public";
        private const string SecretSetting = "Test.Secret";

        private readonly ISettingUiAppService _service;
        private readonly FakePermissionChecker _permissionChecker = new();
        private ISettingManager _settingManager;

        public SettingUiAppServiceAuthorization_Tests()
        {
            _service = GetRequiredService<ISettingUiAppService>();
        }

        protected override void AfterAddApplication(IServiceCollection services)
        {
            services.AddSingleton<IPermissionChecker>(_permissionChecker);

            var settingDefinitionManager = Substitute.For<ISettingDefinitionManager>();
            var publicSetting = new SettingDefinition(PublicSetting, "public")
                .WithProperty(SettingUiConst.Group1, "General")
                .WithProperty(SettingUiConst.Group2, "Basic");
            // Hidden unless SettingUi.Secured and SettingUi.Secured.Connection are granted.
            var secretSetting = new SettingDefinition(SecretSetting, "secret")
                .WithProperty(SettingUiConst.Group1, "Secured")
                .WithProperty(SettingUiConst.Group2, "Connection");
            settingDefinitionManager.GetAllAsync().Returns(new List<SettingDefinition>
            {
                publicSetting,
                secretSetting
            });
            settingDefinitionManager.GetOrNullAsync(PublicSetting).Returns(publicSetting);
            settingDefinitionManager.GetOrNullAsync(SecretSetting).Returns(secretSetting);
            services.AddSingleton(settingDefinitionManager);

            _settingManager = Substitute.For<ISettingManager>();
            services.AddSingleton(_settingManager);

            services.AddSingleton(Substitute.For<ISettingProvider>());
        }

        private void GrantSettingPageOnly()
        {
            _permissionChecker.GrantedPermissions.Add(SettingUiPermissions.ShowSettingPage);
        }

        private void GrantSecuredGroups()
        {
            GrantSettingPageOnly();
            _permissionChecker.GrantedPermissions.Add(TestSettingUiPermissionDefinitionProvider.SecuredGroup);
            _permissionChecker.GrantedPermissions.Add(TestSettingUiPermissionDefinitionProvider.SecuredConnectionGroup);
        }

        private Task ShouldNotHaveWrittenAnySettingAsync()
        {
            return _settingManager.DidNotReceiveWithAnyArgs().SetAsync(default, default, default, default, default);
        }

        [Fact]
        public async Task Reset_Should_Require_The_Setting_Page_Permission()
        {
            await Should.ThrowAsync<AbpAuthorizationException>(
                () => _service.ResetSettingValuesAsync(new List<string> { PublicSetting }));

            await ShouldNotHaveWrittenAnySettingAsync();
        }

        [Fact]
        public async Task Reset_Should_Reset_Visible_Settings_And_Skip_Undefined_Names()
        {
            GrantSettingPageOnly();

            await _service.ResetSettingValuesAsync(new List<string> { PublicSetting, "Not.A.Defined.Setting" });

            await _settingManager.Received(1).SetForCurrentTenantAsync(PublicSetting, null);
            await _settingManager.Received(1).SetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<string>(), Arg.Any<bool>());
        }

        [Fact]
        public async Task Reset_Should_Reject_A_Setting_The_User_Is_Not_Shown()
        {
            GrantSettingPageOnly();

            await Should.ThrowAsync<AbpAuthorizationException>(
                () => _service.ResetSettingValuesAsync(new List<string> { PublicSetting, SecretSetting }));

            // Nothing is reset, not even the visible setting requested before the hidden one.
            await ShouldNotHaveWrittenAnySettingAsync();
        }

        [Fact]
        public async Task Reset_Should_Reset_A_Setting_Its_Group_Permissions_Show()
        {
            GrantSecuredGroups();

            await _service.ResetSettingValuesAsync(new List<string> { PublicSetting, SecretSetting });

            await _settingManager.Received(1).SetForCurrentTenantAsync(PublicSetting, null);
            await _settingManager.Received(1).SetForCurrentTenantAsync(SecretSetting, null);
        }
    }
}
