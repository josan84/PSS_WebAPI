using System.Text.Json;

namespace Insight.PermissionsDealer
{
    public static class PermissionAssistant
    {
        private static RbacRequest x;
        public static async Task<bool> Allow(RbacRequest requestInput, Uri? url)
        {
            x = requestInput;

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
                    
                    return responseDataItems.Exists(Match);
                }
            }
        }

        private static bool Match(DataSourceResponse obj)
        {
            if (x.Input.Role == obj.Role)
            {
                foreach (var p in obj.Permissions)
                {
                    if (x.Input.Action == p.Action && x.Input.Resource == p.Resource)
                        return true;
                }
            }
            return false;
        }
    }
}