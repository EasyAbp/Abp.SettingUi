using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using EasyAbp.Abp.SettingUi.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace EasyAbp.Abp.SettingUi.Pages
{
    public abstract class SettingUiPage : AbpPage
    {
        [RazorInject]
        public IHtmlLocalizer<SettingUiResource> L { get; set; }
    }
}
