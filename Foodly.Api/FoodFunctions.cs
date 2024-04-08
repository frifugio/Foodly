using Foodly.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Foodly.Api
{
    public class FoodFunctions
    {
        private readonly ILogger<FoodFunctions> _logger;
        private readonly CosmosClient cosmosClient;
        private readonly Container cosmosContainer;

        public FoodFunctions(ILogger<FoodFunctions> logger)
        {
            _logger = logger;

            cosmosClient ??= new CosmosClient(Environment.GetEnvironmentVariable("CosmosDBConnectionString"));
            cosmosContainer ??= cosmosClient.GetDatabase(Environment.GetEnvironmentVariable("MyDatabase"))
                                            .GetContainer(Environment.GetEnvironmentVariable("MyContainer"));
        }

        [Function("GetAllFoods")]
        public static IActionResult GetAllFoods(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetAllFoods")]
            HttpRequest req,
            [CosmosDBInput(
                databaseName: "%MyDatabase%",
                containerName: "%MyContainer%",
                Connection = "CosmosDBConnectionString")]
            IReadOnlyCollection<Food> foodList)
        {
            return new OkObjectResult(foodList);
        }


        [Function("AddFood")]
        public async Task<IActionResult> AddFood(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "AddFood")]
            HttpRequest req)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                Food data = JsonConvert.DeserializeObject<Food>(requestBody);

                // Create CosmosDB Item
                Food createdItem = await cosmosContainer.CreateItemAsync(
                    item: data,
                    partitionKey: new PartitionKey(data.Name)
                );
                string createdItemRoute = $"https://foodly-cosmos.documents.azure.com/dbs/{Environment.GetEnvironmentVariable("MyDatabase")}/docs/{createdItem.Id}";
                return new CreatedAtRouteResult(createdItemRoute, createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in adding a Food to the DB\nRequest: {requestBody}\nException {ex}");
                return new StatusCodeResult(500);
            }
        }

        [Function("UpdateFood")]
        public async Task<IActionResult> UpdateFood(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateFood/{id}")]
            HttpRequest req,
            string id)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                Food data = JsonConvert.DeserializeObject<Food>(requestBody);
                Food replacedItem = await cosmosContainer.ReplaceItemAsync(
                        item: data,
                        id: id,
                        partitionKey: new PartitionKey(data.Name)
                    );

                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in updating a Food to the DB\nID: {id}\nRequest: {requestBody}\nException {ex}");
                return new StatusCodeResult(500);
            }
        }

        [Function("DeleteFood")]
        public async Task<IActionResult> DeleteFood(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteFood/{id}")]
            HttpRequest req,
            [CosmosDBInput(
                databaseName: "%MyDatabase%",
                containerName: "%MyContainer%",
                Connection = "CosmosDBConnectionString")]
            IReadOnlyCollection<Food> foodList,
            string id)
        {
            try
            {
                var food = foodList.FirstOrDefault(f => f.Id.ToString() == id);
                if (food == null)
                {
                    _logger.LogError($"Food {id} not found. Delete operation impossible");
                    return new NotFoundResult();
                }

                var response = await cosmosContainer.DeleteItemAsync<Food>(id, new PartitionKey(food.Name));

                return new StatusCodeResult((int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in updating a Food to the DB\nID: {id}\nException {ex}");
                return new StatusCodeResult(500);
            }
        }
    }
}
