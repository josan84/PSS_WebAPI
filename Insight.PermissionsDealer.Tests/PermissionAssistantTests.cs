using Microsoft.Extensions.Logging;
using Moq;

namespace Insight.PermissionsDealer.Tests
{
    public class PermissionAssistantTests
    {
        [Fact]
        public void CustomerReadPortfolioIsAllowed()
        {
            // Arrange
            var permissionAssistant = new PermissionAssistant();
            var rbacRequest = new RbacRequest();
            rbacRequest.Input = new RbacPermissionRequest
            {
                Role = "Customer",
                Action = "Read",
                Resource = "Portfolio"
            };

            var uri = new Uri("https://raw.githubusercontent.com/josan84/PSS_DataSource/main/PSS-architecture-permissions.json");

            var loggerMock = new Mock<ILogger>();

            // Act and Assert
            Assert.True(permissionAssistant.Allow(rbacRequest, uri, loggerMock.Object).Result);
        }

        [Fact]
        public void CustomerUpdatePortfolioIsNotAllowed()
        {
            // Arrange
            var permissionAssistant = new PermissionAssistant();
            var rbacRequest = new RbacRequest();
            rbacRequest.Input = new RbacPermissionRequest
            {
                Role = "Customer",
                Action = "Update",
                Resource = "Portfolio"
            };

            var uri = new Uri("https://raw.githubusercontent.com/josan84/PSS_DataSource/main/PSS-architecture-permissions.json");

            var loggerMock = new Mock<ILogger>();

            // Act and Assert
            Assert.False(permissionAssistant.Allow(rbacRequest, uri, loggerMock.Object).Result);
        }
    }
}