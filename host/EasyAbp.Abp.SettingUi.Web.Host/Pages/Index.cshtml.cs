using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace EasyAbp.Abp.SettingUi.Pages
{
    public class IndexModel : SettingUiPageModel
    {
        public void OnGet()
        {
            
        }

        public async Task OnPostLoginAsync()
        {
            await HttpContext.ChallengeAsync("oidc");
        }
    }
}