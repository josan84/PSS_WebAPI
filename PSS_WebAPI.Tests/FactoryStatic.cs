using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace PSS_WebAPI.Tests
{
    internal class FactoryStatic
    {
        internal static HttpClient CreateHttpClient()
        {
            var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                // ... Configure test services
            });

            return application.CreateClient();
        }
    }
}