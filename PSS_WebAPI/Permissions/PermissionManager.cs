using System.Security.Claims;
using Insight.PermissionsDealer;

namespace PSS_WebAPI.Permissions
{
    public class PermissionManager : IPermissionManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPermissionAssistant _permissionAssistant;
        private readonly ILogger _logger;

        public PermissionManager(IHttpClientFactory httpClientFactory, IPermissionAssistant permissionAssistant, ILogger<PermissionManager> logger)
        {
            _httpClientFactory = httpClientFactory;
            _permissionAssistant = permissionAssistant;
            _logger = logger;

            _logger.LogInformation("Hello 1");
        }
        public async Task<bool> AssertPermissionRequirementAsync(PermissionRequirement permissionRequirement, ClaimsPrincipal claimsPrincipal)
        {
            _logger.LogInformation("Asserting Permissions");

            var userName = claimsPrincipal.Identity != null ? claimsPrincipal.Identity.Name : string.Empty;

            var roleClaimUri = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            var userRole = claimsPrincipal.FindFirstValue(roleClaimUri) ?? string.Empty;

            var rbacRequest = new RbacRequest
            {
                Role = userRole,
                User = userName != null ? userName : string.Empty,
                Action = permissionRequirement.Action,
                Resource = permissionRequirement.Resource
            };

            return _permissionAssistant.Allow(rbacRequest, _httpClientFactory.CreateClient("DataSourceRepo").BaseAddress, _logger).Result;
        }
    }
}