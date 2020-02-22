using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Components;
using Volo.Abp.DependencyInjection;

namespace MyAbpApp.Web
{
    [Dependency(ReplaceServices = true)]
    public class MyAbpAppBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "MyAbpApp";
    }
}
