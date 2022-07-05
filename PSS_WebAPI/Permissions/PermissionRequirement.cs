using Microsoft.AspNetCore.Authorization;

namespace PSS_WebAPI.Permissions
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(string resource, string action)
        {
            Resource = resource;
            Action = action;
        }

        public string Resource { get; }
        public string Action { get; }
    }
}