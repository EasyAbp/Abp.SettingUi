using EasyAbp.Abp.SettingUi.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace EasyAbp.Abp.SettingUi.Web.Pages
{
    /* Inherit your UI Pages from this class. To do that, add this line to your Pages (.cshtml files under the Page folder):
     * @inherits EasyAbp.Abp.SettingUi.Web.Pages.SettingUiPage
     */
    public abstract class SettingUiPage : AbpPage
    {
        [RazorInject]
        public IHtmlLocalizer<SettingUiResource> L { get; set; }
    }
}
