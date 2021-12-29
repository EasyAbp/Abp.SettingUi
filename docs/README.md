[中文](/docs/README_zh-Hans.md)

# Abp.SettingUi

[![ABP version](https://img.shields.io/badge/dynamic/xml?style=flat-square&color=yellow&label=abp&query=//PackageReference[@Include=%27Volo.Abp.Ddd.Domain%27]/@Version&url=https://raw.githubusercontent.com/EasyAbp/Abp.SettingUi/develop/src/EasyAbp.Abp.SettingUi.Domain/EasyAbp.Abp.SettingUi.Domain.csproj)](https://abp.io)
[![NuGet](https://img.shields.io/nuget/v/EasyAbp.Abp.SettingUi.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.SettingUi.Domain.Shared)
[![NuGet Download](https://img.shields.io/nuget/dt/EasyAbp.Abp.SettingUi.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.SettingUi.Domain.Shared)
[![Discord online](https://badgen.net/discord/online-members/S6QaezrCRq?label=Discord)](https://discord.gg/S6QaezrCRq)
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

## Online Demo

We have launched an online demo for this module: [https://settingui.samples.easyabp.io](https://settingui.samples.easyabp.io)

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

        ``` json
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

In this way, when SettingUi enumerates the settings, if a permission in the form of `SettingUi.Group1.Group2` is found, the Group2 in Group1 will only be displayed after the permission is explicitly granted.

Of course, we can also continue to add a permission to precisely control a specified setting, such as "System" -> "Password" -> "Required Length", we need to add a permission with the setting name as the suffix, the code is as follows:
``` csharp
public override void Define(IPermissionDefinitionContext context)
{
    ...
    var requiredLength = passwordGroup.AddChild("SettingUi.System.Password.Abp.Identity.Password.RequiredLength", L("Permission:SettingUi.System.Password.RequiredLength"));    // Add display permission of Abp.Identity.Password.RequiredLength
}
```

In this way, when SettingUi enumerates the settings, if a permission in the form of `SettingUi.Group1.Group2.SettingName` is found, the setting in Group2 in Group1 will only be displayed after the permission is explicitly granted.


Through the above three-level permission definition way, we can arbitrarily control the display of settings in SettingUi.

The following figure is a screenshot of Setting Ui permissions, and the displayed result:

![setting_permission](/docs/images/setting_permission.png)

> For ABP's permission system, please see [the document](https://docs.abp.io/en/abp/latest/Authorization)