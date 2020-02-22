using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Components;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.Abp.SettingUi
{
    [Dependency(ReplaceServices = true)]
    public class SettingUiBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "SettingUi";
    }
}
