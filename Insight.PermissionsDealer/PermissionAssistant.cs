using System.Text.Json;

namespace Insight.PermissionsDealer
{
    public class PermissionAssistant : IPermissionAssistant
    {
        private RbacRequest _rbacRequest;
       
        public async Task<bool> Allow(RbacRequest requestInput, Uri? url)
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
                    
                    return responseDataItems == null ? false : responseDataItems.Exists(Match);
                }
            }
        }

        private bool Match(DataSourceResponse obj)
        {
            if (_rbacRequest.Input.Role == obj.Role)
            {
                foreach (var p in obj.Permissions)
                {
                    if (_rbacRequest.Input.Action == p.Action && _rbacRequest.Input.Resource == p.Resource)
                        return true;
                }
            }
            return false;
        }
    }
}