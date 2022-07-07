using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PSS_WebAPI.Tests
{

    public class GetPortfoliosIntegrationTests
    {
        [Fact]
        public async void GivenHttpClient_GetFreePortfolios()
        {
            // Given
            HttpClient httpClient = FactoryStatic.CreateHttpClient();

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("freeportfolios");
            httpResponseMessage.EnsureSuccessStatusCode();

            var contentString = await httpResponseMessage.Content.ReadAsStringAsync();

            // Then
            Assert.Equal(@"[""abcde, bcedf, xwxyir""]", contentString);
        }

        [Fact]
        public async void GivenHttpClient_GetPortfolios_IsUnauthorized()
        {
            // Given
            HttpClient httpClient = FactoryStatic.CreateHttpClient();

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("portfolios");

            // Then
            Assert.Throws<HttpRequestException>(() => httpResponseMessage.EnsureSuccessStatusCode());
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponseMessage.StatusCode);
        }
    }
}