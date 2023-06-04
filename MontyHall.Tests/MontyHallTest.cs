using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace MontyHall.Tests
{
    public class MontyHallTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MontyHallTest(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;
        
        [Fact]
        private async void ShouldHitPercentageBiggerThanSixtySixPercent()
        {
            // Arrange
            var handler = new MontyHallHandler();
            var numOfSimulation = 1000000;

            // Act
            var response = await handler.Handler(new APIGatewayProxyRequest
            {
                QueryStringParameters = new Dictionary<string, string>
                {
                    { "NumOfSimulations", numOfSimulation.ToString() },
                    { "AlwaysChangeChoice", "true" }
                }
            }, null!);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            var montyHallResponse = JsonConvert.DeserializeObject<MontyHallResponse>(response.Body);
            Assert.NotNull(montyHallResponse);
            Assert.True(montyHallResponse.HitPercentage > 0.66);
            var results = JsonConvert.SerializeObject(montyHallResponse.Results);
            _testOutputHelper.WriteLine($"Hit percentage: {montyHallResponse.HitPercentage * 100}%");
            _testOutputHelper.WriteLine(results);
        }
    }
}
