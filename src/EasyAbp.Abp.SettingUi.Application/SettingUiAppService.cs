using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Authorization;
using EasyAbp.Abp.SettingUi.Dto;
using EasyAbp.Abp.SettingUi.Extensions;
using EasyAbp.Abp.SettingUi.Localization;
using EasyAbp.Abp.SettingUi.Options;
using EasyAbp.Abp.SettingUi.Validation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Json;
using Volo.Abp.Localization;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.Timing;
using Volo.Abp.Validation;
using Volo.Abp.VirtualFileSystem;

namespace EasyAbp.Abp.SettingUi
{
    [Authorize(SettingUiPermissions.ShowSettingPage)]
    public class SettingUiAppService : ApplicationService, ISettingUiAppService
    {
        private readonly AbpSettingUiOptions _options;
        private readonly IStringLocalizer<SettingUiResource> _localizer;
        private readonly IStringLocalizerFactory _factory;
        private readonly IVirtualFileProvider _fileProvider;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly ISettingManager _settingManager;
        private readonly ITimezoneProvider _timezoneProvider;
        private readonly ICurrentTimezoneProvider _currentTimezoneProvider;
        private readonly IPermissionDefinitionManager _permissionDefinitionManager;

        public SettingUiAppService(
            IOptions<AbpSettingUiOptions> options,
            IStringLocalizer<SettingUiResource> localizer,
            IStringLocalizerFactory factory,
            IVirtualFileProvider fileProvider,
            IJsonSerializer jsonSerializer,
            ISettingDefinitionManager settingDefinitionManager,
            ISettingManager settingManager,
            ITimezoneProvider timezoneProvider,
            ICurrentTimezoneProvider currentTimezoneProvider,
            IPermissionDefinitionManager permissionDefinitionManager)
        {
            _options = options.Value;
            _localizer = localizer;
            _factory = factory;
            _fileProvider = fileProvider;
            _jsonSerializer = jsonSerializer;
            _settingDefinitionManager = settingDefinitionManager;
            _settingManager = settingManager;
            _timezoneProvider = timezoneProvider;
            _currentTimezoneProvider = currentTimezoneProvider;
            _permissionDefinitionManager = permissionDefinitionManager;

            LocalizationResource = typeof(SettingUiResource);
            ObjectMapperContext = typeof(AbpSettingUiApplicationModule);
        }

        public virtual async Task<List<SettingGroup>> GroupSettingDefinitionsAsync()
        {
            if (!await AuthorizationService.IsGrantedAsync(SettingUiPermissions.ShowSettingPage))
            {
                throw new AbpAuthorizationException("Authorization failed! No SettingUi policy granted.");
            }

            // Merge all setting properties into one dictionary
            var settingProperties = GetMergedSettingPropertiesAsync();

            var definedSettingUiPermissions = (await _permissionDefinitionManager.GetPermissionsAsync())
                .Where(p => p.Name.StartsWith(SettingUiPermissions.GroupName))
                .ToList();

            // Set properties of the setting definitions
            var settingDefinitions = await SetSettingDefinitionPropertiesAsync(settingProperties, definedSettingUiPermissions);

            // Group the setting definitions
            var groups = new List<SettingGroup>();
            foreach (var settingGroup in settingDefinitions
                .GroupBy(sd => sd.Properties[SettingUiConst.Group1].ToString())
                .Select(grp => new SettingGroup
                {
                    GroupName = grp.Key,
                    GroupDisplayName = _localizer[grp.Key!],
                    SettingInfos = grp.ToList(),
                    Permission = $"{SettingUiPermissions.GroupName}.{grp.Key}"
                }))
            {
                var definedSettingUiGroupPermission = definedSettingUiPermissions.FirstOrDefault(p => p.Name == settingGroup.Permission);
                if (definedSettingUiGroupPermission == null
                    || await AuthorizationService.IsGrantedAsync(definedSettingUiGroupPermission.Name) //Group1 permission check
                )
                {
                    var settingInfoGroups = settingGroup.SettingInfos
                        .GroupBy(sd => sd.Properties[SettingUiConst.Group2].ToString())
                        .Select(grp => new
                        {
                            Permission = $"{SettingUiPermissions.GroupName}.{settingGroup.GroupName}.{grp.Key}",
                            SettingInfoList = grp.ToList()
                        });

                    var settingInfos = new List<SettingInfo>();
                    foreach (var settingInfoGroup in settingInfoGroups)
                    {
                        if (definedSettingUiGroupPermission == null
                            || definedSettingUiGroupPermission.Children.All(p =>
                                p.Name != settingInfoGroup.Permission) || await AuthorizationService.IsGrantedAsync(settingInfoGroup.Permission) //Group2 permission check
                        )
                        {
                            settingInfos.AddRange(settingInfoGroup.SettingInfoList);
                        }
                    }

                    settingGroup.SettingInfos = settingInfos;
                    groups.Add(settingGroup);
                }
            }

            if (_options.DisableDefaultGroup)
            {
                // remove the default group
                var defaultGroup = groups.Find(x => x.GroupName == SettingUiConst.DefaultGroup);
                if (defaultGroup is not null)
                {
                    groups.Remove(defaultGroup);
                }
            }

            return groups;
        }

        public virtual async Task SetSettingValuesAsync(Dictionary<string, string> settingValues)
        {
            // Only the settings the caller is shown can be changed; computed once for the whole request.
            var visibleSettingInfos = await GetVisibleSettingInfosAsync();

            // Check every posted setting before writing any, so a rejected request changes nothing.
            var pendingValues = new List<KeyValuePair<SettingDefinition, string>>();

            foreach (var kv in settingValues)
            {
                // The key of the settingValues is in camel_Case, like "setting_Abp_Localization_DefaultLanguage",
                // change it to "Abp.Localization.DefaultLanguage" form
                string pascalCaseName = kv.Key.ToPascalCase();
                if (!pascalCaseName.StartsWith(SettingUiConst.FormNamePrefix))
                {
                    continue;
                }

                string name = pascalCaseName.RemovePreFix(SettingUiConst.FormNamePrefix).UnderscoreToDot();
                var setting = await _settingDefinitionManager.GetOrNullAsync(name);
                if (setting == null)
                {
                    continue;
                }

                CheckSettingIsVisible(visibleSettingInfos, setting);

                // new value is null.
                if (kv.Value.IsNullOrEmpty())
                {
                    // it's an encrypted setting value, and it's currently on the tenant side.
                    if (setting.IsEncrypted && CurrentTenant.IsAvailable)
                    {
                        // don't update.
                        continue;
                    }
                }

                var value = kv.Value;
                var definition = visibleSettingInfos[setting.Name];

                if (definition.Properties.TryGetValue(SettingUiConst.Type, out var type) &&
                    ((string)type).Equals("dateTime", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (DateTime.TryParse(value, out var dateTime))
                    {
                        // If the DateTime has no timezone info (most cases from input)
                        if (dateTime.Kind == DateTimeKind.Unspecified)
                        {
                            // Try to get user's timezone
                            var userTz = _currentTimezoneProvider.TimeZone;
                            if (!userTz.IsNullOrWhiteSpace())
                            {
                                try
                                {
                                    var tzInfo = _timezoneProvider.GetTimeZoneInfo(userTz);
                                    // Treat the input as user's local time and convert to UTC
                                    value = TimeZoneInfo.ConvertTimeToUtc(dateTime, tzInfo).ToString("O");
                                }
                                catch
                                {
                                    // skip handling this...
                                }
                            }
                            else
                            {
                                value = Clock.Normalize(dateTime).ToString("O");
                            }
                        }
                        else
                        {
                            value = Clock.Normalize(dateTime).ToString("O");
                        }
                    }
                }

                pendingValues.Add(new KeyValuePair<SettingDefinition, string>(setting, value));
            }

            // Validate all values before writing any, so an invalid value saves nothing.
            var validationErrors = new List<ValidationResult>();
            foreach (var pendingValue in pendingValues)
            {
                await ValidateSettingValueAsync(pendingValue.Key, pendingValue.Value, validationErrors);
            }

            if (validationErrors.Any())
            {
                throw new AbpValidationException(validationErrors);
            }

            foreach (var pendingValue in pendingValues)
            {
                await SetSettingAsync(pendingValue.Key, pendingValue.Value);
            }
        }

        public virtual async Task ResetSettingValuesAsync(List<string> settingNames)
        {
            // Same authorization as showing the page: only the settings the caller is shown can be reset.
            var visibleSettingInfos = await GetVisibleSettingInfosAsync();

            // Check every requested name before resetting any, so a rejected request changes nothing.
            var settings = new List<SettingDefinition>();
            foreach (var name in settingNames)
            {
                var setting = await _settingDefinitionManager.GetOrNullAsync(name);
                if (setting == null)
                {
                    continue;
                }

                CheckSettingIsVisible(visibleSettingInfos, setting);
                settings.Add(setting);
            }

            foreach (var setting in settings)
            {
                await SetSettingAsync(setting, null); // use fallback value
            }
        }

        /// <summary>
        /// Returns the settings the current user is shown by <see cref="GroupSettingDefinitionsAsync"/>, by name.
        /// Throws <see cref="AbpAuthorizationException"/> if the user may not see the setting page at all.
        /// </summary>
        protected virtual async Task<Dictionary<string, SettingInfo>> GetVisibleSettingInfosAsync()
        {
            return (await GroupSettingDefinitionsAsync())
                .SelectMany(group => group.SettingInfos)
                .ToDictionary(settingInfo => settingInfo.Name);
        }

        /// <summary>
        /// A defined setting the page does not show the current user (hidden by a group or setting permission,
        /// by <see cref="AbpSettingUiOptions.DisableDefaultGroup"/> or by
        /// <see cref="AbpSettingUiOptions.ExcludeInVisibleToClientSettings"/>) cannot be changed either.
        /// </summary>
        protected virtual void CheckSettingIsVisible(Dictionary<string, SettingInfo> visibleSettingInfos, SettingDefinition setting)
        {
            if (!visibleSettingInfos.ContainsKey(setting.Name))
            {
                throw new AbpAuthorizationException($"Authorization failed! The setting '{setting.Name}' is not available to the current user.");
            }
        }

        /// <summary>
        /// Runs every registered <see cref="ISettingUiValueValidator"/> on a value
        /// <see cref="SetSettingValuesAsync"/> is about to write, and adds their errors to <paramref name="errors"/>.
        /// </summary>
        protected virtual async Task ValidateSettingValueAsync(SettingDefinition setting, [CanBeNull] string value,
            List<ValidationResult> errors)
        {
            var validators = LazyServiceProvider.LazyGetRequiredService<IEnumerable<ISettingUiValueValidator>>();

            var context = new SettingUiValueValidationContext(setting, value);
            foreach (var validator in validators)
            {
                await validator.ValidateAsync(context);
            }

            errors.AddRange(context.Errors);
        }

        protected virtual Task SetSettingAsync(SettingDefinition setting, [CanBeNull] string value)
        {
            if (setting.Providers.Any(p => p == UserSettingValueProvider.ProviderName))
            {
                return _settingManager.SetForCurrentUserAsync(setting.Name, value);
            }

            if (setting.Providers.Any(p => p == GlobalSettingValueProvider.ProviderName))
            {
                return _settingManager.SetGlobalAsync(setting.Name, value);
            }

            return ShouldManageAsGlobal(setting)
                ? _settingManager.SetGlobalAsync(setting.Name, value)
                : _settingManager.SetForCurrentTenantAsync(setting.Name, value);
        }

        protected virtual IDictionary<string, IDictionary<string, string>> GetMergedSettingPropertiesAsync()
        {
            return _fileProvider
                .GetDirectoryContents(SettingUiConst.SettingPropertiesFileFolder)
                .Where(x => x.Name.EndsWith(".json"))
                .Select(content =>
                    _jsonSerializer.Deserialize<IDictionary<string, IDictionary<string, string>>>(content.ReadAsString()))
                .SelectMany(dict => dict)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        protected virtual async Task<List<SettingInfo>> SetSettingDefinitionPropertiesAsync(IDictionary<string, IDictionary<string, string>> settingProperties, IList<PermissionDefinition> permissionDefinitions)
        {
            var settingInfos = new List<SettingInfo>();
            var settingDefinitions = (await _settingDefinitionManager.GetAllAsync())
                .WhereIf(_options.ExcludeInVisibleToClientSettings, setting => setting.IsVisibleToClients);
            foreach (var settingDefinition in settingDefinitions)
            {
				var si =  await CreateSettingInfoAsync(settingDefinition);

                if (settingProperties.ContainsKey(si.Name))
                {
                    // This Setting is defined in the property file,
                    // set its property values from the dictionary
                    var properties = settingProperties[si.Name];
                    foreach (var kv in properties)
                    {
                        // Do not assign the property if it has already been set by the user.
                        if (!si.Properties.ContainsKey(kv.Key))
                        {
							si.Properties[kv.Key] =  kv.Value;
                        }
                    }
                }

                // Default group1: Others
                if (!si.Properties.ContainsKey(SettingUiConst.Group1))
                {
                    si.Properties[SettingUiConst.Group1] = SettingUiConst.DefaultGroup;
                }

                // Default group2: Others
                if (!si.Properties.ContainsKey(SettingUiConst.Group2))
                {
					si.Properties[SettingUiConst.Group2] =  SettingUiConst.DefaultGroup;
                }

                // Default type: text
                if (!si.Properties.ContainsKey(SettingUiConst.Type))
                {
                    si.Properties[SettingUiConst.Type] = SettingUiConst.DefaultType;
                }

                // Setting permission check, once the groups are known
                var permissionName = GetSettingPermissionName(si);
                var definedPermission = permissionDefinitions.FirstOrDefault(p => p.Name == permissionName);
                if (definedPermission == null)
                {
                    WarnAboutMisnamedSettingPermission(si, permissionName, permissionDefinitions);
                }
                else
                {
                    si.Permission = definedPermission.Name;
                    if (!await AuthorizationService.IsGrantedAsync(si.Permission))
                    {
                        continue;
                    }
                }

                settingInfos.Add(si);
            }

            return settingInfos;
        }

        /// <summary>
        /// The name of the permission that, when defined, must be granted to show a setting:
        /// <c>SettingUi.{Group1}.{Group2}.{SettingName}</c>, for example
        /// <c>SettingUi.System.Password.Abp.Identity.Password.RequiredLength</c>.
        /// </summary>
        protected virtual string GetSettingPermissionName(SettingInfo settingInfo)
        {
            return $"{SettingUiPermissions.GroupName}.{settingInfo.Properties[SettingUiConst.Group1]}." +
                   $"{settingInfo.Properties[SettingUiConst.Group2]}.{settingInfo.Name}";
        }

        /// <summary>
        /// Up to version 2.10, a setting permission was any permission whose name ended with the setting name.
        /// A permission that still relies on that no longer applies, so log it for the application to rename.
        /// </summary>
        protected virtual void WarnAboutMisnamedSettingPermission(SettingInfo settingInfo, string expectedPermissionName,
            IList<PermissionDefinition> permissionDefinitions)
        {
            var misnamedPermission = permissionDefinitions.FirstOrDefault(p => p.Name.EndsWith("." + settingInfo.Name));
            if (misnamedPermission != null)
            {
                Logger.LogWarning(
                    "The permission {PermissionName} ends with the name of the setting {SettingName} but is not applied to it: " +
                    "a setting permission must be named {ExpectedPermissionName} (SettingUi.{{Group1}}.{{Group2}}.{{SettingName}}).",
                    misnamedPermission.Name, settingInfo.Name, expectedPermissionName);
            }
        }

        protected virtual async Task<SettingInfo> CreateSettingInfoAsync(SettingDefinition settingDefinition)
        {
            string name = settingDefinition.Name;
            string displayName;
            if (settingDefinition.DisplayName is FixedLocalizableString fls && fls.Value == settingDefinition.Name)
            {
                displayName = _localizer[$"DisplayName:{settingDefinition.Name}"];
            }
            else
            {
                displayName = settingDefinition.DisplayName.Localize(_factory);
            }

            string description;
            if (settingDefinition.Description == null)
            {
                string descName = $"Description:{settingDefinition.Name}";
                description = _localizer[descName];
                if (description == descName)
                {
                    // No localized description found
                    description = String.Empty;
                }
            }
            else
            {
                description = settingDefinition.Description.Localize(_factory);
            }

            var value = await GetSettingValueAsync(settingDefinition);

            var si = new SettingInfo
            {
                Name = name,
                DisplayName = displayName,
                Description = description,
                Value = value,
                Properties = new ExtraPropertyDictionary(),
            };

            // Copy properties from SettingDefinition
            foreach (var property in settingDefinition.Properties)
            {
                si.Properties[property.Key] = property.Value;
            }

            return si;
        }

        protected virtual async Task<string> GetSettingValueAsync(SettingDefinition settingDefinition)
        {
            /* Hide default/global value for tenants if the setting item is encrypted. */
            if (settingDefinition.IsEncrypted && CurrentTenant.IsAvailable)
            {
                return await _settingManager.GetOrNullForCurrentTenantAsync(settingDefinition.Name, false);
            }

            return ShouldManageAsGlobal(settingDefinition)
                ? await _settingManager.GetOrNullGlobalAsync(settingDefinition.Name)
                : await SettingProvider.GetOrNullAsync(settingDefinition.Name);
        }

        protected virtual bool ShouldManageAsGlobal(SettingDefinition settingDefinition)
        {
            // todo: settingDefinition.Providers.Count != 0 can be improved.
            return !CurrentTenant.IsAvailable &&
                   _options.ManageGlobalSettingsOnHostSide &&
                   settingDefinition.Providers.Count == 0;
        }
    }
}