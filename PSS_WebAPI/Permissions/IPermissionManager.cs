using System.Security.Claims;

namespace PSS_WebAPI.Permissions
{
    public interface IPermissionManager
    {
        Task<bool> AssertPermissionRequirementAsync (PermissionRequirement permissionRequirement, 
            ClaimsPrincipal claimsPrincipal);
    }
}
