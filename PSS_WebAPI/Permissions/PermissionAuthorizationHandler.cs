using Microsoft.AspNetCore.Authorization;

namespace PSS_WebAPI.Permissions
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionManager _permissionManager;

        public PermissionAuthorizationHandler(IPermissionManager permissionManager)
        {
            _permissionManager = permissionManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                context.Fail();
                return;
            }

           var result = await _permissionManager.AssertPermissionRequirementAsync(requirement, context.User);

            if (result)
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
            return;
        }
    }
}
