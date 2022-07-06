using Microsoft.Extensions.Logging;

namespace Insight.PermissionsDealer
{
    public interface IPermissionAssistant
    {
        Task<bool> Allow(RbacRequest requestInput, Uri? url, ILogger logger);
    }
}