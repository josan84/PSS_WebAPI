using System.Security.Claims;
using Insight.PermissionsDealer;

namespace PSS_WebAPI.Permissions
{
    public class PermissionManager : IPermissionManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPermissionAssistant _permissionAssistant;

        public PermissionManager(IHttpClientFactory httpClientFactory, IPermissionAssistant permissionAssistant)
        {
            _httpClientFactory = httpClientFactory;
            _permissionAssistant = permissionAssistant;
        }
        public async Task<bool> AssertPermissionRequirementAsync(PermissionRequirement permissionRequirement, ClaimsPrincipal claimsPrincipal)
        {
            var userName = claimsPrincipal.Identity != null ? claimsPrincipal.Identity.Name : string.Empty;

            var roleClaimUri = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            var userRole = claimsPrincipal.FindFirstValue(roleClaimUri).ToLower() ?? string.Empty;

            var rbacPermissionRequest = new RbacPermissionRequest
            {
                Role = userRole.ToLower(),
                User = userName != null ? userName.ToLower() : string.Empty,
                Action = permissionRequirement.Action.ToLower(),
                Resource = permissionRequirement.Resource.ToLower()
            };

            var rbacRequest = new RbacRequest
            {
                Input = rbacPermissionRequest
            };

            return _permissionAssistant.Allow(rbacRequest, _httpClientFactory.CreateClient("DataSourceRepo").BaseAddress).Result;
        }
    }
}