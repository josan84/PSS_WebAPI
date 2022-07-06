namespace Insight.PermissionsDealer
{
    public class RbacRequest
    {
        public string Role { get; set; }
        public string User { get; set; }
        public string Action { get; set; }
        public string Resource { get; set; }
    }
}
