using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Insight.PermissionsDealer
{
    public class PermissionAssistant : IPermissionAssistant
    {
        private RbacRequest _rbacRequest;
       
        public async Task<bool> Allow(RbacRequest requestInput, Uri? url, ILogger logger)
        {
            _rbacRequest = requestInput;

            using (var client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url)) 
                {
                    string dataSourceContent = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    List<DataSourceResponse>? responseDataItems = JsonSerializer.Deserialize<List<DataSourceResponse>>(dataSourceContent, options);
                    
                    if (responseDataItems != null && responseDataItems.Exists(Match))
                    {
                        logger.LogInformation("Allowed");
                        return true;
                    }

                    logger.LogInformation("Not Allowed");
                    return false;
                }
            }
        }

        private bool Match(DataSourceResponse obj)
        {
            if (string.Equals(_rbacRequest.Role, obj.Role, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var p in obj.Permissions)
                {
                    if (string.Equals(_rbacRequest.Action, p.Action, StringComparison.OrdinalIgnoreCase)
                         && string.Equals(_rbacRequest.Resource, p.Resource, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            return false;
        }
    }
}