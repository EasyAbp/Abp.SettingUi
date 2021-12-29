# Abp.SettingUi

[![ABP version](https://img.shields.io/badge/dynamic/xml?style=flat-square&color=yellow&label=abp&query=//PackageReference[@Include=%27Volo.Abp.Ddd.Domain%27]/@Version&url=https://raw.githubusercontent.com/EasyAbp/Abp.SettingUi/develop/src/EasyAbp.Abp.SettingUi.Domain/EasyAbp.Abp.SettingUi.Domain.csproj)](https://abp.io)
[![NuGet](https://img.shields.io/nuget/v/EasyAbp.Abp.SettingUi.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.SettingUi.Domain.Shared)
[![NuGet Download](https://img.shields.io/nuget/dt/EasyAbp.Abp.SettingUi.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.SettingUi.Domain.Shared)
[![Discord online](https://badgen.net/discord/online-members/S6QaezrCRq?label=Discord)](https://discord.gg/S6QaezrCRq)
[![GitHub stars](https://img.shields.io/github/stars/EasyAbp/Abp.SettingUi?style=social)](https://www.github.com/EasyAbp/Abp.SettingUi)

一个用来管理[ABP](http://abp.io)设置的模块

![demo](/docs/images/demo.png)

> 如果你在使用 ABP v2.1.1 之前的版本, 请查看[Abp.SettingManagement.Mvc.UI](https://github.com/wakuflair/Abp.SettingManagement.Mvc.UI)

## 功能

* 通过UI管理ABP设置的值
* 支持本地化
* 设置分组
* 为不同设置显示适当的控件
* 可通过权限控制设置的显示

## 在线演示

我们为这个模块创建了一个在线演示: [https://settingui.samples.easyabp.io](https://settingui.samples.easyabp.io)

## 安装

### 使用[AbpHelper](https://github.com/EasyAbp/AbpHelper.CLI) (推荐)

在你的ABP项目的根文件夹中运行以下命令:

> abphelper module add EasyAbp.Abp.SettingUi -acshlw

### 手动安装包

1. 安装以下 NuGet 包.

    * EasyAbp.Abp.SettingUi.Application
    * EasyAbp.Abp.SettingUi.Application.Contracts
    * EasyAbp.Abp.SettingUi.Domain.Shared
    * EasyAbp.Abp.SettingUi.HttpApi
    * EasyAbp.Abp.SettingUi.HttpApi.Client (只有 [分层结构](https://docs.abp.io/en/abp/latest/Startup-Templates/Application#tiered-structure) 才需要)
    * EasyAbp.Abp.SettingUi.Web

1. 添加 `DependsOn(typeof(AbpSettingUiXxxModule))` 属性来配置模块依赖. ([帮助](https://github.com/EasyAbp/EasyAbpGuide/blob/master/How-To.md#add-module-dependencies))

### 配置本地化资源

为了让SettingUi模块使用应用程序的本地化资源, 我们需要将它们添加进`SettingUiResource`:


* `MyAbpApp.Domain.Shared` 项目 - `MyAbpAppDomainSharedModule` 类

    ``` csharp
    Configure<AbpLocalizationOptions>(options =>
    {
        ...
        options.Resources
            .Get<SettingUiResource>()
            .AddVirtualJson("/Localization/MyAbpApp");
    });
    ```

## 使用

1. 授权 ("Setting UI" - "Show Setting Page")

    ![permission](/docs/images/permission.png)

1. 刷新浏览器, 然后你就可以使用 "Administration" - "Settings" 菜单来看见所有ABP内置的设置了

## 管理自定义设置

除了ABP自定义设置以外, 你也可以使用这个模块来管理你自己的设置.

1. 定义一个设置

    * `MyAbpApp.Domain` 项目 - `Settings/MyAbpAppSettingDefinitionProvider` 类

        ``` csharp
        public class MyAbpAppSettingDefinitionProvider : SettingDefinitionProvider
        {
            public override void Define(ISettingDefinitionContext context)
            {
                context.Add(
                    new SettingDefinition(
                        "Connection.Ip", // 设置的名称
                        "127.0.0.1", // 默认值
                        L("DisplayName:Connection.Ip"), // 显示名称
                        L("Description:Connection.Ip") // 描述
                    ));
            }

            private static LocalizableString L(string name)
            {
                return LocalizableString.Create<MyAbpAppResource>(name);
            }
        }
        ```

        * 设置的名称为"Connection.Ip"
        * 提供了一个默认值: "127.0.0.1"
        * 使用帮助方法 `L` 为 `显示名称` 和 `描述` 赋予了可本地化的字符串. 格式 "DisplayName:{SettingName}" 是ABP推荐的形式.


        > ABP的设置系统, 请参见 [设置文档](https://docs.abp.io/en/abp/latest/Settings)

1. 定义本地化资源, 出于演示目的, 我们定义了英语和简体中文的本地化资源

    * `MyAbpApp.Domain.Shared` 项目

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

1. 重新启动应用程序, 我们可以看到设置显示了, 并且本地化也正常工作

    ![custom-setting](/docs/images/custom-setting.png)

## 分组

你可能注意到我们的自定义设置显示在"其它"标签, "其它"卡片中, 这些是默认的分组, 分别称之为"Group1"和"Group2"

![group](/docs/images/group.png)

那么我们如何自定义这些设置的分组呢? 有两种方式:

1. 使用 `WithProperty` 方法

    `WithProperty` 方法是由ABP`SettingDefinition`类提供的一个方法, 我们可以直接在设置定义中使用它:

    * `MyAbpApp.Domain` 项目 - `Settings/MyAbpAppSettingDefinitionProvider` 类

        ``` csharp
        context.Add(
            new SettingDefinition(
                    "Connection.Ip", // 设置名称
                    "127.0.0.1", // 默认值
                    L("DisplayName:Connection.Ip"), // 显示名称
                    L("Description:Connection.Ip") // 描述
                )
                .WithProperty(SettingUiConst.Group1, "Server")
                .WithProperty(SettingUiConst.Group2, "Connection")
        );
        ```

        * 常量 `Group1` 和 `Group2` 定义在 `SettingUiConst`类中
        * 设置 "Group1" 为 "Server", "Group2" 为 "Connection"

    然后我们应该为这两个分组名字提供本地化资源:

    * `MyAbpApp.Domain.Shared` 项目

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

    重新启动应用程序查看分组名称是否正确设置

    ![group-name](/docs/images/group-name.png)

1. 使用设置属性文件

    另一种分组方式是使用设置分组文件, 该方式由SettingUi模块提供. 当你不太容易修改设置的定义, 或者你想将分组信息汇集在一个单独的位置时, 这种方式很有用.

    为了演示这种方式, 让我们定义一个新设置:

    * `MyAbpApp.Domain` 项目 - `Settings/MyAbpAppSettingDefinitionProvider` 类

        ``` json
        new SettingDefinition(
            "Connection.Port",
            8080.ToString(),
            L("DisplayName:Connection.Port"),
            L("Description:Connection.Port")
        )
        ```
    > 为这个设置添加本地化的步骤省略了.

    然后我们需要创建一个新的任意名字的JSON文件, 但是路径必须为"/SettingProperties", 这是因为SettingUi模块将会从这个路径下查找设置属性文件.

    * `MyAbpApp.Domain.Shared` 项目 - `/SettingProperties/MySettingProperties.json` 文件

        ``` json
        {
            "Connection.Port": {
                "Group1": "Server",
                "Group2": "Connection"
            }
        }
        ```

        * 设置名称 `Connection.Port` 做为JSON对象的键
        * 使用 "Group1" 和 "Group2" 来设置分组名称

    * 重新启动应用程序来查看新分组的设置

        ![group-by-setting-property-file](/docs/images/group-by-setting-property-file.png)

## 设置类型

默认情况下, 一个设置的值是字符串类型, 将会在UI中渲染为一个文本输入控件. 我们可以简单地提供一个设置属性"Type"来定制它:

   * `MyAbpApp.Domain.Shared` 项目 - `/SettingProperties/MySettingProperties.json` 文件

        ``` json
        {
            "Connection.Port": {
                "Group1": "Server",
                "Group2": "Connection",
                "Type": "number"
            }
        }
        ```

        * "Connection.Port" 设置类型为 "number"

不用重新启动应用程序, 只需要按下F5来刷新浏览器, 你可以立即看到效果:

![type-number](/docs/images/type-number.png)

现在输入的类型变更为了"数字", 并且前端的验证也生效了.

> 设置类型也可以通过 `WithProperty` 方法来配置, 如 `WithProperty("Type", "number")`

目前SettingUi支持以下几种设置类型:

* text (默认)
* number
* checkbox
* select
  * 需要一个额外属性 "Options" 来提供选项, 是一个使用竖线(|)分隔的字符串

    ``` json
    "Connection.Protocol": {
        "Group1": "Server",
        "Group2": "Connection",
        "Type": "select",
        "Options": "|HTTP|TCP|RDP|FTP|SFTP"
    }

    ```

    渲染结果:

    ![selection](/docs/images/selet.png)

到这里教程就结束了. 通过本教程, 你应该可以轻松地使用SettingUi来管理你的设置了. 教程的源码可以在[sample文件夹](https://github.com/EasyAbp/Abp.SettingUi/tree/master/sample)中找到.

# 本地化

SettingUi模块使用ABP的本地化系统来显示设置的本地化信息. 现在支持的语言有:

* 英语
* 简体中文
* 土耳其语

本地化资源存放在`EasyAbp.Abp.SettingUi.Domain.Shared`项目的`/Localization/SettingUi`中.

你可以添加更多的资源文件来让这个模块支持更多语言. 欢迎PR :blush: .
> ABP的本地化系统, 请查看[文档](https://docs.abp.io/en/abp/latest/Localization)

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

您也可以使用`SettingUiPermissions.GroupName`变量, 作用与上方代码相同, 如

``` csharp
public override void Define(IPermissionDefinitionContext context)
{
    var settingUiPage = context.GetPermissionOrNull(SettingUiPermissions.ShowSettingPage);  // 取得ShowSettingPage权限
    var systemGroup = settingUiPage.AddChild(SettingUiPermissions.GroupName + ".System", L("Permission:SettingUi.System")); // 添加控制 Group1: System 的权限
}
```

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

> 关于ABP中权限系统, 请查看[该文档](https://docs.abp.io/en/abp/latest/Authorization)


