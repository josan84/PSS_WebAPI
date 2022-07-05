namespace Insight.PermissionsDealer
{
    public class DataSourceResponse
    {
        // public List<PermissionData> PermissionsData { get; set; }
       // public PermissionData PermissionsData { get; set; }

       // public class PermissionData
      //  {
            public string Role { get; set; }
            public List<Permission> Permissions { get; set; }
      //  }
        public class Permission
        {
            public string Action { get; set; }
            public string Resource { get; set; }
        }
    }
}