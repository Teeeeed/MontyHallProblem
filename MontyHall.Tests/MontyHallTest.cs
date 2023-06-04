using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace MontyHall.Tests
{
    public class MontyHallTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MontyHallTest(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;
        
        [Fact]
        private void ShouldHitPercentageBiggerThanSixtySixPercent()
        {
            // Arrange
            var numOfSimulation = 1000000;
            var montyHallHandler = new MontyHallHandler();
            var rand = new Random();
            
            // Act
            var simulationResults = Enumerable.Range(0, numOfSimulation)
                .Select(_ => montyHallHandler.SimulateMontyHall(rand.Next(0, 3), true))
                .ToList();
            var hitCount = simulationResults.Count(result => result.Hit);
            
            // Assert
            var hitPercentage = (double)hitCount / numOfSimulation;
            _testOutputHelper.WriteLine($"Hit percentage: {hitPercentage * 100}%");
            Assert.True(hitPercentage > 0.66);
        }
    }
}
