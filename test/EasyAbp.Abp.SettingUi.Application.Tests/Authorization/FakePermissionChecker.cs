using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;

namespace EasyAbp.Abp.SettingUi.Authorization
{
    /// <summary>
    /// Grants exactly the permissions in <see cref="GrantedPermissions"/>, whoever the caller is.
    /// </summary>
    public class FakePermissionChecker : IPermissionChecker
    {
        public HashSet<string> GrantedPermissions { get; } = new();

        public Task<bool> IsGrantedAsync(string name)
        {
            return Task.FromResult(GrantedPermissions.Contains(name));
        }

        public Task<bool> IsGrantedAsync(ClaimsPrincipal claimsPrincipal, string name)
        {
            return IsGrantedAsync(name);
        }

        public Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names)
        {
            var result = new MultiplePermissionGrantResult();
            foreach (var name in names.Distinct())
            {
                result.Result[name] = GrantedPermissions.Contains(name)
                    ? PermissionGrantResult.Granted
                    : PermissionGrantResult.Prohibited;
            }

            return Task.FromResult(result);
        }

        public Task<MultiplePermissionGrantResult> IsGrantedAsync(ClaimsPrincipal claimsPrincipal, string[] names)
        {
            return IsGrantedAsync(names);
        }
    }
}
