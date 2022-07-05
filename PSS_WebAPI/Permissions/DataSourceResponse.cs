namespace OPAStyraWebAPI.Permissions
{
    public class DataSourceResponse
    {
        public string Role { get; set; }
        public class Permissions
        {
            public string Action { get; set; }
            public string Resource { get; set; }
        }
    }
}