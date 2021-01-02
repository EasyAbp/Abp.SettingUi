# Abp.SettingUi

[![ABP version](https://img.shields.io/badge/dynamic/xml?style=flat-square&color=yellow&label=abp&query=%2F%2FProject%2FPropertyGroup%2FAbpVersion&url=https%3A%2F%2Fraw.githubusercontent.com%2FEasyAbp%2FAbp.SettingUi%2Fmaster%2FDirectory.Build.props)](https://abp.io)
[![NuGet](https://img.shields.io/nuget/v/EasyAbp.Abp.SettingUi.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.SettingUi.Domain.Shared)
[![NuGet Download](https://img.shields.io/nuget/dt/EasyAbp.Abp.SettingUi.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.SettingUi.Domain.Shared)
[![GitHub stars](https://img.shields.io/github/stars/EasyAbp/Abp.SettingUi?style=social)](https://www.github.com/EasyAbp/Abp.SettingUi)

An [ABP](http://abp.io) module used to manage ABP settings

![demo](/docs/images/demo.png)

> If you are using ABP version <2.1.1, please see [Abp.SettingManagement.Mvc.UI](https://github.com/wakuflair/Abp.SettingManagement.Mvc.UI)

## Features

* Manage ABP setting values via UI
* Support localization
* Group settings
* Display settings with appropriate input controls

## Online Demo

We have launched an online demo for this module: [https://settingui.samples.easyabp.io](https://settingui.samples.easyabp.io)

## Installation

### Add ABP packages with [AbpHelper](https://github.com/EasyAbp/AbpHelper.CLI) (Recommended)

Run following command in your ABP project root folder:

> abphelper module add EasyAbp.Abp.SettingUi -ashlw

### Add ABP packages manually

1. Install the following NuGet packages.

    * EasyAbp.Abp.SettingUi.Application
    * EasyAbp.Abp.SettingUi.Domain.Shared
    * EasyAbp.Abp.SettingUi.HttpApi
    * EasyAbp.Abp.SettingUi.HttpApi.Client (Only [Tiered structure](https://docs.abp.io/en/abp/latest/Startup-Templates/Application#tiered-structure) is needed)
    * EasyAbp.Abp.SettingUi.Web

1. Add `DependsOn(typeof(AbpSettingUiXxxModule))` attribute to configure the module dependencies. ([see how](https://github.com/EasyAbp/EasyAbpGuide/blob/master/How-To.md#add-module-dependencies))

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

1. Add permissions ("Setting UI" - "Show Setting Page") to the roles you want.

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

For now SettingUi support following setting types:

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

This is the end of the tutorial. Through this tutorial, you should be able to easily manage your settings using SettingUi. The source of the tutorial can be found in the [sample folder](https://github.com/EasyAbp/Abp.SettingUi/tree/master/sample)

# Localization

The SettingUI module uses ABP's localization system to display the localization information of the settings.The languages currently supported are:

* en
* zh-Hans

The localization resource files are under `/Localization/SettingUi` of the `EasyAbp.Abp.SettingUi.Domain.Shared` project.

You can add more resource files to make this module support more languages. Welcome PRs :blush: .
> For ABP's localization system, please see [the document](https://docs.abp.io/en/abp/latest/Localization)

# 权限

SettingUi通过检查`SettingUi.ShowSettingPage`权限,来控制是否显示SettingUi的页面.

只要赋予了该权限, 那么系统中所有的设置都可以通过SettingUi来修改.

但有些时候, 我们不想让用户在SettingUi中看到某些设置, 这可以通过定义特定的权限来实现这个目的.

比如我们需要对用户隐藏"系统"分组, 那么需要在`SettingUi.ShowSettingPage`下添加一个子权限, 权限的名字为`SettingUi.System`. 代码如下:

``` csharp
public override void Define(IPermissionDefinitionContext context)
{
    var settingUiPage = context.GetPermissionOrNull(SettingUiPermissions.ShowSettingPage);  // 取得ShowSettingPage权限
    var systemGroup = settingUiPage.AddChild("SettingUi.System", L("Permission:SettingUi.System")); // 添加控制 Group1: System 的权限
}
```

这样当SettingUi遍历设置时, 如果发现有`SettingUi.Group1`形式的权限, 则只有显式的赋予该权限后, 分组Group1才会显示.

我们可以继续添加对Group2控制的权限, 如"系统" -> "密码"分组, 需要继续添加后缀为Group2的权限, 代码如下:

``` csharp
public override void Define(IPermissionDefinitionContext context)
{
    ...
    var passwordGroup = systemGroup.AddChild("SettingUi.System.Password", L("Permission:SettingUi.System.Password"));   // 添加控制 Group2: Password 的权限
}
```

这样当SettingUi遍历设置时, 如果发现有`SettingUi.Group1.Group2`形式的权限, 则只有显示的赋予该权限后, 分组Group1中的Group2才会显示.

当然, 我们也可继续添加精确控制某一设置的权限, 如"系统" -> "密码" -> "要求长度", 需要继续添加后缀为设置名称的权限, 代码如下:
``` csharp
public override void Define(IPermissionDefinitionContext context)
{
    ...
    var requiredLength = passwordGroup.AddChild("SettingUi.System.Password.Abp.Identity.Password.RequiredLength", L("Permission:SettingUi.System.Password.RequiredLength"));    // 添加控制设置Abp.Identity.Password.RequiredLength的权限
}
```

这样当SettingUi遍历设置时, 如果发现有`SettingUi.Group1.Group2.SettingName`形式的权限, 则只有显示的赋予该权限后, 分组Group1中的Group2中的SettingName才会显示.


通过以上3级的权限定义方式, 我们就可以在SettingUi中任意控制设置的显示了.

下图是Setting Ui权限的截图, 和显示的结果:

![setting_permission](/docs/images/setting_permission.png)

> 关于ABP中权限系统, 请参见[该文档](https://docs.abp.io/en/abp/latest/Authorization)


