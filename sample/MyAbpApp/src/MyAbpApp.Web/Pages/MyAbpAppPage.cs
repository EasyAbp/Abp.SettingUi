using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using MyAbpApp.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace MyAbpApp.Web.Pages
{
    /* Inherit your UI Pages from this class. To do that, add this line to your Pages (.cshtml files under the Page folder):
     * @inherits MyAbpApp.Web.Pages.MyAbpAppPage
     */
    public abstract class MyAbpAppPage : AbpPage
    {
        [RazorInject]
        public IHtmlLocalizer<MyAbpAppResource> L { get; set; }
    }
}
