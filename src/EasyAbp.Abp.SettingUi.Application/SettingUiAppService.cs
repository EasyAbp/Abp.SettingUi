using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyAbp.Abp.SettingUi.Authorization;
using EasyAbp.Abp.SettingUi.Dto;
using EasyAbp.Abp.SettingUi.Extensions;
using EasyAbp.Abp.SettingUi.Localization;
using EasyAbp.Abp.SettingUi.Options;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
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
using Volo.Abp.VirtualFileSystem;

namespace EasyAbp.Abp.SettingUi
{
	public class SettingUiAppService : ApplicationService, ISettingUiAppService
	{
		private readonly AbpSettingUiOptions _options;
		private readonly IStringLocalizer<SettingUiResource> _localizer;
		private readonly IStringLocalizerFactory _factory;
		private readonly IVirtualFileProvider _fileProvider;
		private readonly IJsonSerializer _jsonSerializer;
		private readonly ISettingDefinitionManager _settingDefinitionManager;
		private readonly ISettingManager _settingManager;
		private readonly IPermissionDefinitionManager _permissionDefinitionManager;

		public SettingUiAppService(
			IOptions<AbpSettingUiOptions> options,
			IStringLocalizer<SettingUiResource> localizer,
			IStringLocalizerFactory factory,
			IVirtualFileProvider fileProvider,
			IJsonSerializer jsonSerializer,
			ISettingDefinitionManager settingDefinitionManager,
			ISettingManager settingManager,
			IPermissionDefinitionManager permissionDefinitionManager)
		{
			_options = options.Value;
			_localizer = localizer;
			_factory = factory;
			_fileProvider = fileProvider;
			_jsonSerializer = jsonSerializer;
			_settingDefinitionManager = settingDefinitionManager;
			_settingManager = settingManager;
			_permissionDefinitionManager = permissionDefinitionManager;
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

				await SetSettingAsync(setting, kv.Value); // todo: needs permission check?
			}
		}

		public virtual async Task ResetSettingValuesAsync(List<string> settingNames)
		{
			foreach (var name in settingNames)
			{
				var setting = await _settingDefinitionManager.GetOrNullAsync(name);
				if (setting == null)
				{
					continue;
				}

				await SetSettingAsync(setting, null); // use fallback value
			}
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

			//Default
			return _settingManager.SetForCurrentTenantAsync(setting.Name, value);
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
			var settingDefinitions = await _settingDefinitionManager.GetAllAsync();
			foreach (var settingDefinition in settingDefinitions)
			{
				var si =  await CreateSettingInfoAsync(settingDefinition);
				
				var definedPermission = permissionDefinitions.FirstOrDefault(p => p.Name.EndsWith(si.Name));
				if (definedPermission != null)
				{
					si.Permission = definedPermission.Name;
					if (!await AuthorizationService.IsGrantedAsync(si.Permission))
					{
						continue;
					}
				}

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

				settingInfos.Add(si);
			}

			return settingInfos;
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

			/* Hide default/global value for tenants if the setting item is encrypted. */
			var value = settingDefinition.IsEncrypted && CurrentTenant.IsAvailable
				? await _settingManager.GetOrNullForCurrentTenantAsync(name, false)
				: await SettingProvider.GetOrNullAsync(name);

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
	}
}