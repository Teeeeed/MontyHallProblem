using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json.Converters;
using Amazon.Lambda.Serialization.SystemTextJson;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
namespace MontyHall
{
    public class MontyHallHandler
    {
        public MontyHallHandler() { }

        public Task<APIGatewayProxyResponse> Handler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            bool isNumOfSimulationsInQuery = APIGatewayProxyRequestUtility.TryGetQueryParam(request, "NumOfSimulations", out int numOfSimulations);
            bool isAlwaysChangeChoiceInQuery = APIGatewayProxyRequestUtility.TryGetQueryParam(request, "AlwaysChangeChoice", out bool alwaysChangeChoice);

            if (!(isNumOfSimulationsInQuery && isAlwaysChangeChoiceInQuery))
            {
                throw new Exception("Invalid request, query parameters are missing.");
            }

            var montyHallRequest = new MontyHallRequest
            {
                NumOfSimulations = numOfSimulations,
                AlwaysChangeChoice = alwaysChangeChoice
            };

            var rand = new Random();
            var simulationResults = Enumerable.Range(0, montyHallRequest.NumOfSimulations)
                .Select(_ => SimulateMontyHall(rand.Next(0, 3), montyHallRequest.AlwaysChangeChoice))
                .ToList();
            var hitCount = simulationResults.Count(result => result.Hit);
            var montyHallResponse = new MontyHallResponse
            {
                Results = simulationResults,
                HitPercentage = (double)hitCount / montyHallRequest.NumOfSimulations
            };
            
            return Task.FromResult(new APIGatewayProxyResponse
            {
                StatusCode = HttpStatusCode.OK.GetHashCode(),
                Body = JsonConvert.SerializeObject(montyHallResponse, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = new List<JsonConverter>
                    {
                        new StringEnumConverter()
                    }
                })
            });
        }

        /// <summary><c>SimulateMontyHall</c> simulates a single case of the monty hall problem.</summary>
        /// <param name="carDoor">The door behind which the car is.</param>
        /// <param name="changeChoice"> True if the player changes their initial choice, otherwise false.</param>
        /// <returns>The <see cref="SimulationResult" /> of a single case of the monty hall problem.</returns>
        private SimulationResult SimulateMontyHall(int carDoor, bool changeChoice)
        {
            var simulationResult = new SimulationResult
            {
                ChosenDoorByPlayer = 0,
                Simulation = new List<bool>{false, false, false}
            };
            simulationResult.Simulation[carDoor] = true;

            switch (carDoor)
            {
                case 0:
                    simulationResult.ChosenDoorByPresenter = new Random().Next(1, 2);
                    simulationResult.Hit = !changeChoice;
                    break;
                case 1:
                    simulationResult.ChosenDoorByPresenter = 2;
                    simulationResult.Hit = changeChoice;
                    break;
                case 2:
                    simulationResult.ChosenDoorByPresenter = 1;
                    simulationResult.Hit = changeChoice;
                    break;
            }

            return simulationResult;
        }
    }
}

