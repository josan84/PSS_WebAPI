namespace Insight.PermissionsDealer
{
    public class DataSourceResponse
    {
            public string Role { get; set; }
            public List<Permission> Permissions { get; set; }
        public class Permission
        {
            public string Action { get; set; }
            public string Resource { get; set; }
        }
    }
}