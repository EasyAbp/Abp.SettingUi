[中文](/docs/README_zh-Hans.md)

# Abp.SettingUi

[![ABP version](https://img.shields.io/badge/dynamic/xml?style=flat-square&color=yellow&label=abp&query=%2F%2FProject%2FPropertyGroup%2FAbpVersion&url=https%3A%2F%2Fraw.githubusercontent.com%2FEasyAbp%2F%2FAbp.SettingUi%2Fmain%2FDirectory.Build.props)](https://abp.io)
[![NuGet](https://img.shields.io/nuget/v/EasyAbp.Abp.SettingUi.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.SettingUi.Domain.Shared)
[![NuGet Download](https://img.shields.io/nuget/dt/EasyAbp.Abp.SettingUi.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.SettingUi.Domain.Shared)
[![Discord online](https://badgen.net/discord/online-members/xyg8TrRa27?label=Discord)](https://discord.gg/xyg8TrRa27)
[![GitHub stars](https://img.shields.io/github/stars/EasyAbp/Abp.SettingUi?style=social)](https://www.github.com/EasyAbp/Abp.SettingUi)

An [ABP](http://abp.io) module used to manage ABP settings

![demo](/docs/images/demo.png)

> If you are using ABP version <2.1.1, please see [Abp.SettingManagement.Mvc.UI](https://github.com/wakuflair/Abp.SettingManagement.Mvc.UI)

## Features

* Manage ABP setting values via UI
* Support localization
* Group settings
* Display settings with appropriate input controls
* Control display of settings by permissions
* Save only the values that were changed, and show where each value comes from
* Keep encrypted settings masked until they are asked for

## Installation

### Add ABP packages with [AbpHelper](https://github.com/EasyAbp/AbpHelper.CLI) (Recommended)

Run following command in your ABP project root folder:

> abphelper module add EasyAbp.Abp.SettingUi -acshlw

### Add ABP packages manually

1. Install the following NuGet packages.

    * EasyAbp.Abp.SettingUi.Application
    * EasyAbp.Abp.SettingUi.Application.Contracts
    * EasyAbp.Abp.SettingUi.Domain.Shared
    * EasyAbp.Abp.SettingUi.HttpApi
    * EasyAbp.Abp.SettingUi.HttpApi.Client (Only [Tiered structure](https://docs.abp.io/en/abp/latest/Startup-Templates/Application#tiered-structure) is needed)
    * EasyAbp.Abp.SettingUi.Web

1. Add `DependsOn(typeof(AbpSettingUiXxxModule))` attribute to configure the module dependencies. ([see how](https://github.com/EasyAbp/EasyAbpGuide/blob/master/docs/How-To.md#add-module-dependencies))

### Configure localization resource

In order to let SettingUi module use localization resources from this application, we need to add them to `SettingUiResource`:


* `MyAbpApp.Domain.Shared` project - `MyAbpAppDomainSharedModule` class

    ``` csharp
    Configure<AbpLocalizationOptions>(options =>
    {
        ...
        options.Resources
            .Get<SettingUiResource>()
            .AddVirtualJson("/Localization/MyAbpApp");
    });
    ```

## Usage

1. Grant permission ("Setting UI" - "Show Setting Page")

    ![permission](/docs/images/permission.png)

1. Refresh the browser then you can use "Administration" - "Settings" menu to see all ABP built-in settings

## Skip the InVisibleToClients settings
```
 Configure<AbpSettingUiOptions>(options =>
 {
     //Exclude "IsVisibleToClients = false" Settings
     options.ExcludeInVisibleToClientSettings = true;
 });
```

## Manage custom settings

Beside ABP built-in settings, you can also use this module to manage your own settings.

1. Define a setting

    * `MyAbpApp.Domain` project - `Settings/MyAbpAppSettingDefinitionProvider` class

        ``` csharp
        public class MyAbpAppSettingDefinitionProvider : SettingDefinitionProvider
        {
            public override void Define(ISettingDefinitionContext context)
            {
                context.Add(
                    new SettingDefinition(
                        "Connection.Ip", // Setting name
                        "127.0.0.1", // Default value
                        L("DisplayName:Connection.Ip"), // Display name
                        L("Description:Connection.Ip") // Description
                    ));
            }

            private static LocalizableString L(string name)
            {
                return LocalizableString.Create<MyAbpAppResource>(name);
            }
        }
        ```

        * The setting name is "Connection.Ip"
        * Provide a default value: "127.0.0.1"
        * Set the `DisplayName` and `Description` to a localizable string by using a helper method `L`. The format "DisplayName:{SettingName}" is the convention recommended by ABP

        > For ABP setting system, please see [Settings document](https://docs.abp.io/en/abp/latest/Settings)

1. Define localization resources for the setting, for demonstration purpose, we defined English and Chinese localization resources

    * `MyAbpApp.Domain.Shared` project

      * `Localization/MyAbpApp/en.json`

        ``` json
        {
            "culture": "en",
            "texts": {
                ...
                "DisplayName:Connection.Ip": "IP",
                "Description:Connection.Ip": "The IP address of the server."
            }
        }
        ```

      * `Localization/MyAbpApp/zh-Hans.json`

        ``` json
        {
            "culture": "zh-Hans",
            "texts": {
                ...
                "DisplayName:Connection.Ip": "IP",
                "Description:Connection.Ip": "服务器的IP地址."
            }
        }
        ```

1. Relaunch the application, we can see the setting displayed, and the localization also works

    ![custom-setting](/docs/images/custom-setting.png)

## Grouping

You may notice that our custom setting is displayed in "Others" tab, and "Others" card, these are the default group display names called "Group1" and "Group2" respectively:

![group](/docs/images/group.png)

So how can we custom the group of the setting? There are two ways:

1. Use `WithProperty` method

    The `WithProperty` method is a method provided by ABP `SettingDefinition` class, we can directly use it in setting defining:

    * `MyAbpApp.Domain` project - `Settings/MyAbpAppSettingDefinitionProvider` class

        ``` csharp
        context.Add(
            new SettingDefinition(
                    "Connection.Ip", // Setting name
                    "127.0.0.1", // Default value
                    L("DisplayName:Connection.Ip"), // Display name
                    L("Description:Connection.Ip") // Description
                )
                .WithProperty(SettingUiConst.Group1, "Server")
                .WithProperty(SettingUiConst.Group2, "Connection")
        );
        ```

        * The constants `Group1` and `Group2` are defined in the `SettingUiConst` class
        * Set the "Server" to "Group1", and "Connection" to "Group2"

    Then we should provide the localization resource for these two group names:

    * `MyAbpApp.Domain.Shared` project

      * `Localization/MyAbpApp/en.json`

        ``` json
        {
            "culture": "en",
            "texts": {
                ...
                "Server": "Server",
                "Connection": "Connection"
            }
        }
        ```

      * `Localization/MyAbpApp/zh-Hans.json`

        ``` json
        {
            "culture": "zh-Hans",
            "texts": {
                ...
                "Server": "服务器",
                "Connection": "连接"
            }
        }
        ```

    Relaunch the application and see if the group names are correctly set

    ![group-name](/docs/images/group-name.png)

1. Use setting property file

    Another way of setting group is use the setting property file, which is provided by the SettingUi module. It's useful when you can not easily modify the setting definition, or you want to put the grouping information into one single place.

    For demonstration in this way, let's define a new setting:

    * `MyAbpApp.Domain` project - `Settings/MyAbpAppSettingDefinitionProvider` class

        ``` csharp
        new SettingDefinition(
            "Connection.Port",
            8080.ToString(),
            L("DisplayName:Connection.Port"),
            L("Description:Connection.Port")
        )
        ```
    > The steps of adding localization for this setting are omitted.

    Then we need to create a new json file with arbitrary filename, however the path must be "/SettingProperties", because SettingUi module will look for the setting property files from this path.

    * `MyAbpApp.Domain.Shared` project - `/SettingProperties/MySettingProperties.json` file

        ``` json
        {
            "Connection.Port": {
                "Group1": "Server",
                "Group2": "Connection"
            }
        }
        ```

        * The setting name `Connection.Port` as the key of the JSON object
        * Use "Group1" and "Group2" to set the grouping names

    * Relaunch the application to see the new grouped setting

        ![group-by-setting-property-file](/docs/images/group-by-setting-property-file.png)

## Setting types

By default a setting value is string type, which will be rendered as a text input control in UI. We can custom it simply by providing a setting property "Type":

   * `MyAbpApp.Domain.Shared` project - `/SettingProperties/MySettingProperties.json` file

        ``` json
        {
            "Connection.Port": {
                "Group1": "Server",
                "Group2": "Connection",
                "Type": "number"
            }
        }
        ```

        * Set the "Connection.Port" setting type to "number"

No need to relaunch the application, just press F5 to refresh the browser, you should be able to see the effect immediately:

![type-number](/docs/images/type-number.png)

Now the input type changed to "number", and the frontend validations also work.

> The setting types can also be configured through `WithProperty` method, like `WithProperty("Type", "number")`

For now SettingUi supports following setting types:

* text (default)
* number
* date
* dateTime
* checkbox
* select
  * Needs an additional property "Options" to provide select options, which is a string separated by a vertical bar (|)

    ``` json
    "Connection.Protocol": {
        "Group1": "Server",
        "Group2": "Connection",
        "Type": "select",
        "Options": "|HTTP|TCP|RDP|FTP|SFTP"
    }

    ```

    The render result:

    ![selection](/docs/images/selet.png)

This is the end of the tutorial. Through this tutorial, you should be able to easily manage your settings using SettingUi. The source of the tutorial can be found in the [sample folder](https://github.com/EasyAbp/Abp.SettingUi/tree/master/sample).

## Saving setting values

Each card of the page has its own "Save" button, which posts every box of the card. SettingUi writes only what the user changed:

* A value equal to the one the page showed is skipped, so saving a card never pins a copy of an inherited value (a default, a configuration or Key Vault value, or the host's value seen from a tenant). Values are compared as their type: `True` and `true` are the same checkbox value, and a `number`, `date` or `dateTime` value can come back in another format.
* Line endings are normalized to `\n` before a value is compared and stored. A browser posts a textarea with `\r\n` line endings.
* **An empty box resets the setting**, like the "Reset" button: the stored value is deleted and the setting inherits its value again. An empty string can no longer be stored through the page; this also applies to the empty option of a `select`. Before version 3.1 an empty box stored `""`, which hid every inherited value and broke the consumers of a number setting.

### Encrypted settings

A setting defined with `isEncrypted: true` is shown as a password box, whatever its type, and the setting list (`GroupSettingDefinitionsAsync`) never carries its value: `SettingInfo.Value` is `null` and `SettingInfo.HasValue` tells whether one is set.

* Left empty, the box **keeps** the current value, on the host and on the tenant side alike. Typing a value replaces it. "Reset" deletes it.
* The eye button loads the value the first time it is clicked and shows it. A loaded value that is saved unchanged is skipped like any other.
* A tenant is shown only its own value, never the one of the host, the configuration or the default value.

The value is loaded with `ISettingUiAppService.GetSettingValueAsync(name)`, available as `POST /api/setting-ui/get-setting-value?name={settingName}` and in the C# and JavaScript client proxies (`easyAbp.abp.settingUi.settingUi.getSettingValue`). It requires the same permissions as saving the setting, and it is a `POST` so that ABP writes an audit log for it (ABP does not audit `GET` requests by default).

### Where a value comes from

Each setting shows a badge for the source of its value: *Default*, *Configuration*, *Global* (on the host), *Inherited from host* (a global value seen from a tenant), *Set here*, or *Not set*. A value is *set here* when it is stored by the provider the page saves to: the tenant on the tenant side, the global value on the host when `ManageGlobalSettingsOnHostSide` is enabled (otherwise the host's own value), and the user for a user setting. Such a value has a "Reset" link that resets this setting only.

`SettingInfo.ValueProviderName` carries the name of the provider (`D`, `C`, `G`, `T` or `U`, or `null` when no provider has a value) and `SettingInfo.IsValueSetHere` whether it is the one the page saves to.

> If you override `Pages/Components/SettingUi/Default.cshtml`, render `Partials/_ValueSource.cshtml` next to the label and `Partials/_Encrypted.cshtml` for an encrypted setting, as the module's own view does.

## Validate setting values

The values of the `number`, `date` and `dateTime` types are also validated on the server: a `number` must be a whole number (in the invariant culture, as the browser's number box accepts no decimals by default), and a `date` or `dateTime` must be a valid date. This is done by `SettingUiTypeValueValidator`, which is registered by default. To accept other values, remove it in the `ConfigureServices` method of your module and register your own validator, which can derive from it and override `IsValidNumber` or `IsValidDateTime`:

``` csharp
context.Services.RemoveAll(s => s.ImplementationType == typeof(SettingUiTypeValueValidator));
```

To validate other values on the server before they are saved, implement `ISettingUiValueValidator` (namespace `EasyAbp.Abp.SettingUi.Validation`) and register it in the dependency injection container:

* `MyAbpApp.Application` project - `Settings/ConnectionPortSettingUiValueValidator` class

    ``` csharp
    [ExposeServices(typeof(ISettingUiValueValidator))]
    public class ConnectionPortSettingUiValueValidator : ISettingUiValueValidator, ITransientDependency
    {
        private readonly IStringLocalizer<MyAbpAppResource> _localizer;

        public ConnectionPortSettingUiValueValidator(IStringLocalizer<MyAbpAppResource> localizer)
        {
            _localizer = localizer;
        }

        public Task ValidateAsync(SettingUiValueValidationContext context)
        {
            if (context.SettingDefinition.Name != "Connection.Port")
            {
                return Task.CompletedTask; // Not a setting this validator handles
            }

            if (!int.TryParse(context.Value, out var port) || port < 1 || port > 65535)
            {
                context.Errors.Add(new ValidationResult(
                    _localizer["InvalidPort"], // e.g. "The port must be a number between 1 and 65535."
                    new[] { context.SettingDefinition.Name }));
            }

            return Task.CompletedTask;
        }
    }
    ```

* Every registered validator is called for every value a save is about to write (`context.Value` is the value as it will be stored, or `null` when the box was cleared to reset the setting), so return without adding errors for the settings you do not handle. Only the values the user changed are written, so only those are validated. `context.Type`, `context.Properties` and `context.DisplayName` give the SettingUi type, properties and localized name of the setting.
* The values of one save are all validated before any of them is written. If any validator adds an error, the errors are thrown together as an `AbpValidationException` and nothing is saved; the page shows the messages to the user. A validator may also throw a `UserFriendlyException` or `BusinessException` itself, with the same effect.
* Resetting settings is not validated: it restores the default value of the setting definition.

## Upgrading from 3.0

* `GroupSettingDefinitionsAsync` no longer returns the value of an encrypted setting: use `SettingInfo.HasValue`, and `GetSettingValueAsync` to read it.
* An empty value posted to `SetSettingValuesAsync` resets a setting instead of storing `""`, and an empty value of an encrypted setting keeps it, also on the host (where 3.0 stored `""`). Values equal to the shown ones are not written.
* `number`, `date` and `dateTime` values are validated on the server. A number with decimals is refused: give such a setting another type, or replace `SettingUiTypeValueValidator` (see [Validate setting values](#validate-setting-values)).
* `SettingInfo` has new members (`IsEncrypted`, `HasValue`, `ValueProviderName`, `IsValueSetHere`), and `SettingUiValueValidationContext` new properties (`Type`, `Properties`, `DisplayName`); existing code keeps compiling.
* A value an earlier version stored as `""` shows as an empty box, so it is reset the next time its card is saved (an encrypted one is kept until it is reset). A value stored with `\r\n` line endings stays until it is changed or reset.

# Localization

The SettingUi module uses ABP's localization system to display the localization information of the settings.The languages currently supported are:

* en
* zh-Hans
* tr

The localization resource files are under `/Localization/SettingUi` of the `EasyAbp.Abp.SettingUi.Domain.Shared` project.

You can add more resource files to make this module support more languages. Welcome PRs :blush: .
> For ABP's localization system, please see [the document](https://docs.abp.io/en/abp/latest/Localization)

# Permissions

SettingUi controls whether to display SettingUi's page by checking the `SettingUi.ShowSettingPage` permission.

As long as the permission is granted, all settings in the system can be modified through SettingUi.

But sometimes, we don't want users to see certain settings in SettingUi, which can be achieved by defining specific permissions.

For example, if we need to hide the "system" group from users, then we need to add a child permission of `SettingUi.ShowSettingPage`, the name of the permission is `SettingUi.System`. The code is as follows:

``` csharp
public override void Define(IPermissionDefinitionContext context)
{
    var settingUiPage = context.GetPermissionOrNull(SettingUiPermissions.ShowSettingPage);  // Get ShowSettingPage permission
    var systemGroup = settingUiPage.AddChild("SettingUi.System", L("Permission:SettingUi.System")); // Add display permission of Group1: System
}
```

In this way, when SettingUi enumerates the settings, if a permission in the form of `SettingUi.Group1` is found, the Group1 will only be displayed after the permission is explicitly granted.

You can also use the `SettingUiPermissions.GroupName` variable. The effect is the same as the above code. The code is as follows:

``` csharp
public override void Define(IPermissionDefinitionContext context)
{
    var settingUiPage = context.GetPermissionOrNull(SettingUiPermissions.ShowSettingPage);  // Get ShowSettingPage permission
    var systemGroup = settingUiPage.AddChild(SettingUiPermissions.GroupName + ".System", L("Permission:SettingUi.System")); // Add display permission of Group1: System
}
```

We can continue to add permissions to control Group2, such as "System" -> "Password" group, we need to add a permission with the Group2 name as the suffix, the code is as follows:
``` csharp
public override void Define(IPermissionDefinitionContext context)
{
    ...
    var passwordGroup = systemGroup.AddChild("SettingUi.System.Password", L("Permission:SettingUi.System.Password"));   // Add display permission of Group2: Password
}
```

In this way, when SettingUi enumerates the settings, if a permission in the form of `SettingUi.Group1.Group2` is found, the Group2 in Group1 will only be displayed after the permission is explicitly granted. This applies wherever the permission is defined in the permission tree, also when there is no Group1 permission (before version 3.0 it was only enforced as a child of a defined Group1 permission).

Of course, we can also continue to add a permission to precisely control a specified setting, such as "System" -> "Password" -> "Required Length", we need to add a permission with the setting name as the suffix, the code is as follows:
``` csharp
public override void Define(IPermissionDefinitionContext context)
{
    ...
    var requiredLength = passwordGroup.AddChild("SettingUi.System.Password.Abp.Identity.Password.RequiredLength", L("Permission:SettingUi.System.Password.RequiredLength"));    // Add display permission of Abp.Identity.Password.RequiredLength
}
```

In this way, when SettingUi enumerates the settings, if a permission in the form of `SettingUi.Group1.Group2.SettingName` is found, the setting in Group2 in Group1 will only be displayed after the permission is explicitly granted.

> Name the setting permission exactly `SettingUi.{Group1}.{Group2}.{SettingName}`. Before version 3.0 a setting used the first SettingUi permission whose name merely ended with the setting name, so a permission of one setting could hide or show another (`Ip` and `Server.Ip`, for example). Now the exactly named permission is used when it is defined. Otherwise, for compatibility, every SettingUi permission whose name ends with the setting name must be granted, and a warning asks you to rename it; a setting is unrestricted only when no permission matches at all. Override `SettingUiAppService.GetSettingPermissionName` to use another naming scheme.


Through the above three-level permission definition way, we can arbitrarily control the display of settings in SettingUi.

The same permissions also control which settings can be changed: saving or resetting a setting requires `SettingUi.ShowSettingPage`, and a request that names a setting the user is not shown (hidden by a group or setting permission, `DisableDefaultGroup` or `ExcludeInVisibleToClientSettings`) is rejected as unauthorized without changing anything.

The following figure is a screenshot of Setting Ui permissions, and the displayed result:

![setting_permission](/docs/images/setting_permission.png)

> For ABP's permission system, please see [the document](https://docs.abp.io/en/abp/latest/Authorization)
