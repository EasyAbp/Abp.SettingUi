using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace MyAbpApp.Web
{
    [Dependency(ReplaceServices = true)]
    public class MyAbpAppBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "MyAbpApp";
    }
}
