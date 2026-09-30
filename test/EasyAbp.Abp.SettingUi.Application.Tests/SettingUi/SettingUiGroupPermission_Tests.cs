using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Authorization;
using Microsoft.Extensions.DependencyInjection;
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
    /// A Group2 permission SettingUi.{Group1}.{Group2} is enforced wherever it is defined in the permission tree.
    /// </summary>
    public class SettingUiGroupPermission_Tests : SettingUiApplicationTestBase
    {
        private const string OrphanSetting = "Orphan.Setting";
        private const string SplitSetting = "Split.Setting";
        private const string NestedSetting = "Nested.Setting";

        private readonly ISettingUiAppService _service;
        private readonly FakePermissionChecker _permissionChecker = new();

        public SettingUiGroupPermission_Tests()
        {
            _service = GetRequiredService<ISettingUiAppService>();

            _permissionChecker.GrantedPermissions.Add(SettingUiPermissions.ShowSettingPage);
        }

        protected override void AfterAddApplication(IServiceCollection services)
        {
            services.AddSingleton<IPermissionChecker>(_permissionChecker);

            var settingDefinitionManager = Substitute.For<ISettingDefinitionManager>();
            var settings = new List<SettingDefinition>
            {
                new SettingDefinition(OrphanSetting)
                    .WithProperty(SettingUiConst.Group1, "Orphan")
                    .WithProperty(SettingUiConst.Group2, "Files"),
                new SettingDefinition(SplitSetting)
                    .WithProperty(SettingUiConst.Group1, "Split")
                    .WithProperty(SettingUiConst.Group2, "Files"),
                new SettingDefinition(NestedSetting)
                    .WithProperty(SettingUiConst.Group1, "Nested")
                    .WithProperty(SettingUiConst.Group2, "Files")
            };
            settingDefinitionManager.GetAllAsync().Returns(settings);
            foreach (var setting in settings)
            {
                settingDefinitionManager.GetOrNullAsync(setting.Name).Returns(setting);
            }

            services.AddSingleton(settingDefinitionManager);
            services.AddSingleton(Substitute.For<ISettingManager>());
            services.AddSingleton(Substitute.For<ISettingProvider>());
        }

        private async Task<List<string>> GetVisibleSettingNamesAsync()
        {
            return (await _service.GroupSettingDefinitionsAsync())
                .SelectMany(group => group.SettingInfos)
                .Select(settingInfo => settingInfo.Name)
                .ToList();
        }

        [Fact]
        public async Task A_Group2_Permission_Without_A_Group1_Permission_Should_Be_Enforced()
        {
            (await GetVisibleSettingNamesAsync()).ShouldNotContain(OrphanSetting);

            _permissionChecker.GrantedPermissions.Add(P.OrphanFilesGroup);

            (await GetVisibleSettingNamesAsync()).ShouldContain(OrphanSetting);
        }

        [Fact]
        public async Task A_Group2_Permission_That_Is_Not_A_Child_Of_The_Group1_Permission_Should_Be_Enforced()
        {
            _permissionChecker.GrantedPermissions.Add(P.SplitGroup);

            (await GetVisibleSettingNamesAsync()).ShouldNotContain(SplitSetting);

            _permissionChecker.GrantedPermissions.Add(P.SplitFilesGroup);

            (await GetVisibleSettingNamesAsync()).ShouldContain(SplitSetting);
        }

        [Fact]
        public async Task A_Group2_Permission_Under_Its_Group1_Permission_Should_Need_Both()
        {
            _permissionChecker.GrantedPermissions.Add(P.NestedFilesGroup);

            (await GetVisibleSettingNamesAsync()).ShouldNotContain(NestedSetting);

            _permissionChecker.GrantedPermissions.Remove(P.NestedFilesGroup);
            _permissionChecker.GrantedPermissions.Add(P.NestedGroup);

            (await GetVisibleSettingNamesAsync()).ShouldNotContain(NestedSetting);

            _permissionChecker.GrantedPermissions.Add(P.NestedFilesGroup);

            (await GetVisibleSettingNamesAsync()).ShouldContain(NestedSetting);
        }
    }
}
