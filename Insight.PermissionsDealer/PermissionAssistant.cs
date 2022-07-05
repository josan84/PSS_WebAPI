using System.Text.Json;

namespace Insight.PermissionsDealer
{
    public static class PermissionAssistant
    {
        public static async Task<bool> Allow(string permissionRequestInput, Uri? url)
        {
            using (var client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url)) 
                {
                    string dataSourceContent = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var responseDataItems = JsonSerializer.Deserialize<List<DataSourceResponse>>(dataSourceContent, options);
                    
                    return permissionRequestInput == responseDataItems[0].Permissions[0].Action;
                }
            }
        }
    }
}