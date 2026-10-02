# Abp.SettingUi

[![ABP version](https://img.shields.io/badge/dynamic/xml?style=flat-square&color=yellow&label=abp&query=%2F%2FProject%2FPropertyGroup%2FAbpVersion&url=https%3A%2F%2Fraw.githubusercontent.com%2FEasyAbp%2F%2FAbp.SettingUi%2Fmain%2FDirectory.Build.props)](https://abp.io)
[![NuGet](https://img.shields.io/nuget/v/EasyAbp.Abp.SettingUi.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.SettingUi.Domain.Shared)
[![NuGet Download](https://img.shields.io/nuget/dt/EasyAbp.Abp.SettingUi.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.SettingUi.Domain.Shared)
[![Discord online](https://badgen.net/discord/online-members/xyg8TrRa27?label=Discord)](https://discord.gg/xyg8TrRa27)
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
* 只保存被修改的值, 并显示每个值的来源
* 加密的设置默认隐藏, 需要时才加载

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

## 跳过`InVisibleToClients`设置
```
 Configure<AbpSettingUiOptions>(options =>
 {
     //Exclude "IsVisibleToClients = false" Settings
     options.ExcludeInVisibleToClientSettings = true;
 });
```
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
* date
* dateTime
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

## 保存设置值

页面上的每张卡片都有自己的"保存"按钮, 它会提交卡片中的所有输入框. SettingUi只写入用户修改过的值:

* 与页面显示的值相同的值会被跳过, 因此保存卡片永远不会固定一份继承来的值的副本(默认值, 配置或Key Vault中的值, 或租户看到的宿主值). 值按其类型比较: `True`和`true`是同一个复选框值, `number`, `date`或`dateTime`的值也可能以另一种格式提交回来.
* 在比较和存储之前, 换行符会统一为`\n`. 浏览器提交文本域(textarea)时使用`\r\n`换行.
* **清空输入框即重置该设置**, 与"重置"按钮效果相同: 已存储的值被删除, 设置重新继承它的值. 通过页面已无法存储空字符串; `select`的空选项同样如此. 3.1版本之前, 空输入框会存储`""`, 这会遮蔽所有继承的值, 并导致数字类型设置的使用方出错.

### 加密的设置

使用`isEncrypted: true`定义的设置无论类型如何都显示为密码框, 并且设置列表(`GroupSettingDefinitionsAsync`)永远不包含它的值: `SettingInfo.Value`为`null`, `SettingInfo.HasValue`表示是否已设置值.

* 输入框留空则**保持**当前值, 宿主端和租户端都是如此. 输入新值则替换它. "重置"会删除它.
* 第一次点击眼睛按钮时会加载并显示该值. 加载后未修改就保存的值, 和其他值一样会被跳过.
* 租户只能看到自己的值, 永远看不到宿主, 配置或默认值.

该值通过`ISettingUiAppService.GetSettingValueAsync(name)`加载, 对应的接口为`POST /api/setting-ui/get-setting-value?name={settingName}`, C#和JavaScript客户端代理中也提供了该方法(`easyAbp.abp.settingUi.settingUi.getSettingValue`). 它需要与保存该设置相同的权限; 使用`POST`是为了让ABP为它记录审计日志(ABP默认不审计`GET`请求).

### 值的来源

每个设置都会显示一个标记, 表示其值的来源: *默认值*, *配置*, *全局*(宿主端), *继承自宿主*(租户看到的全局值), *在此设置*或*未设置*. 当值由页面保存的目标提供者存储时, 即为*在此设置*: 租户端为租户, 宿主端启用`ManageGlobalSettingsOnHostSide`时为全局值(否则为宿主自己的值), 用户设置则为用户. 这样的值旁边有一个"重置"链接, 只重置这一个设置.

`SettingInfo.ValueProviderName`是提供者的名称(`D`, `C`, `G`, `T`或`U`, 没有提供者有值时为`null`), `SettingInfo.IsValueSetHere`表示它是否就是页面保存的目标提供者.

> 如果你重写了`Pages/Components/SettingUi/Default.cshtml`, 请像模块自己的视图一样, 在标签旁渲染`Partials/_ValueSource.cshtml`, 并为加密的设置渲染`Partials/_Encrypted.cshtml`.

## 校验设置值

`number`, `date`和`dateTime`类型的值也会在服务端校验: `number`必须是整数(使用固定区域性(invariant culture), 因为浏览器的数字输入框默认不接受小数), `date`或`dateTime`必须是有效的日期. 这由默认注册的`SettingUiTypeValueValidator`完成. 如需接受其他值, 请在你的模块的`ConfigureServices`方法中移除它, 再注册你自己的校验器(可以继承它并重写`IsValidNumber`或`IsValidDateTime`):

``` csharp
context.Services.RemoveAll(s => s.ImplementationType == typeof(SettingUiTypeValueValidator));
```

如果需要在保存前于服务端校验其他设置值, 请实现`ISettingUiValueValidator`(命名空间`EasyAbp.Abp.SettingUi.Validation`), 并将其注册到依赖注入容器中:

* `MyAbpApp.Application`项目 - `Settings/ConnectionPortSettingUiValueValidator`类

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
                return Task.CompletedTask; // 不是本校验器负责的设置
            }

            if (!int.TryParse(context.Value, out var port) || port < 1 || port > 65535)
            {
                context.Errors.Add(new ValidationResult(
                    _localizer["InvalidPort"], // 例如 "端口必须是1到65535之间的数字."
                    new[] { context.SettingDefinition.Name }));
            }

            return Task.CompletedTask;
        }
    }
    ```

* 每次保存时, 所有已注册的校验器都会收到本次将要写入的每一个值(`context.Value`是即将存储的值, 输入框被清空以重置设置时为`null`), 因此对于不由你负责的设置, 请直接返回, 不要添加错误. 只有用户修改过的值才会被写入, 因此也只有这些值会被校验. `context.Type`, `context.Properties`和`context.DisplayName`提供设置的SettingUi类型, 属性和本地化名称.
* 一次保存中的所有值都会先完成校验, 然后才会写入任何一个值. 只要有校验器添加了错误, 所有错误会合并为一个`AbpValidationException`抛出, 且不会保存任何值; 页面会向用户显示这些错误信息. 校验器也可以自行抛出`UserFriendlyException`或`BusinessException`, 效果相同.
* 重置设置不会触发校验: 重置只是恢复设置定义的默认值.

## 从3.0升级

* `GroupSettingDefinitionsAsync`不再返回加密设置的值: 请使用`SettingInfo.HasValue`, 并通过`GetSettingValueAsync`读取它.
* 提交给`SetSettingValuesAsync`的空值会重置设置而不是存储`""`; 加密设置的空值会保持原值, 宿主端也是如此(3.0会存储`""`). 与显示值相同的值不会被写入.
* `number`, `date`和`dateTime`的值会在服务端校验. 带小数的数字会被拒绝: 请为这样的设置使用其他类型, 或替换`SettingUiTypeValueValidator`(参见[校验设置值](#校验设置值)).
* `SettingInfo`新增了成员(`IsEncrypted`, `HasValue`, `ValueProviderName`, `IsValueSetHere`), `SettingUiValueValidationContext`新增了属性(`Type`, `Properties`, `DisplayName`); 已有代码仍可编译.
* 早期版本存储为`""`的值会显示为空输入框, 因此下次保存所在卡片时会被重置(加密的值则保留到被重置为止). 带有`\r\n`换行符的值会一直保留, 直到被修改或重置.

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

这样当SettingUi遍历设置时, 如果发现有`SettingUi.Group1.Group2`形式的权限, 则只有显示的赋予该权限后, 分组Group1中的Group2才会显示. 无论该权限定义在权限树的什么位置都会生效, 即使没有定义Group1的权限也是如此(3.0版本之前, 只有作为已定义的Group1权限的子权限时才会生效).

当然, 我们也可继续添加精确控制某一设置的权限, 如"系统" -> "密码" -> "要求长度", 需要继续添加后缀为设置名称的权限, 代码如下:
``` csharp
public override void Define(IPermissionDefinitionContext context)
{
    ...
    var requiredLength = passwordGroup.AddChild("SettingUi.System.Password.Abp.Identity.Password.RequiredLength", L("Permission:SettingUi.System.Password.RequiredLength"));    // 添加控制设置Abp.Identity.Password.RequiredLength的权限
}
```

这样当SettingUi遍历设置时, 如果发现有`SettingUi.Group1.Group2.SettingName`形式的权限, 则只有显示的赋予该权限后, 分组Group1中的Group2中的SettingName才会显示.

> 设置权限请精确命名为`SettingUi.{Group1}.{Group2}.{SettingName}`. 3.0版本之前, 设置会使用第一个名称以该设置名结尾的SettingUi权限, 因此一个设置的权限可能会隐藏或显示另一个设置(例如`Ip`和`Server.Ip`). 现在如果定义了精确命名的权限, 就使用该权限. 否则为了兼容, 所有名称以该设置名结尾的SettingUi权限都必须被授予, 同时会记录一条警告提示你重命名; 只有没有任何权限匹配时, 该设置才不受限制. 如需使用其他命名方式, 请重写`SettingUiAppService.GetSettingPermissionName`.


通过以上3级的权限定义方式, 我们就可以在SettingUi中任意控制设置的显示了.

同样的权限也控制哪些设置可以被修改: 保存或重置设置需要`SettingUi.ShowSettingPage`权限, 如果请求中包含了用户看不到的设置(被分组或设置权限、`DisableDefaultGroup`或`ExcludeInVisibleToClientSettings`隐藏), 请求会以未授权被拒绝, 且不会修改任何内容.

下图是Setting Ui权限的截图, 和显示的结果:

![setting_permission](/docs/images/setting_permission.png)

> 关于ABP中权限系统, 请查看[该文档](https://docs.abp.io/en/abp/latest/Authorization)


