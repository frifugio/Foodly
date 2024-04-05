using Foodly.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Foodly.Api
{
    public class FoodFunctions
    {
        private readonly ILogger<FoodFunctions> _logger;
        public FoodFunctions(ILogger<FoodFunctions> logger)
        {
            _logger = logger;
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
        public static async Task<IActionResult> AddFood(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "AddFood")]
            HttpRequest req,
            [CosmosDBInput(
                databaseName: "%MyDatabase%",
                containerName: "%MyContainer%",
                Connection = "CosmosDBConnectionString")]
            IAsyncCollector<Food> foodOut
            )
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                Food data = JsonConvert.DeserializeObject<Food>(requestBody);
                await foodOut.AddAsync(data);

                // Add a JSON document to the output container.
                //var result = await foodOut.AddAsync(new
                //{
                //    // create a random ID
                //    id = System.Guid.NewGuid().ToString(),
                //    // altri campi
                //});

                var databaseName = Environment.GetEnvironmentVariable("MyDatabase");
                var containerName = Environment.GetEnvironmentVariable("MyContainer");
                return new OkResult();
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        [Function("UpdateFood")]
        public static async Task<IActionResult> UpdateFood(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateFood/{id}")]
            HttpRequest req,
            [CosmosDBInput(
                    databaseName: "%MyDatabase%",
                    containerName: "%MyContainer%",
                    Connection = "CosmosDBConnectionString",
                    Id = "{id}")]
            IAsyncCollector<Food> foodOut
            )
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                Food data = JsonConvert.DeserializeObject<Food>(requestBody);
                await foodOut.AddAsync(data);

                var databaseName = Environment.GetEnvironmentVariable("MyDatabase");
                var containerName = Environment.GetEnvironmentVariable("MyContainer");
                return new OkObjectResult(data);
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        //[Function("DeleteFood")]
        //public async Task<IActionResult> DeleteFood(
        //    [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteFood/{id}")]
        //    [CosmosDBInput(
        //        databaseName: "%MyDatabase%",
        //        containerName: "%MyContainer%",
        //        Connection = "CosmosDBConnectionString",
        //        SqlQuery = "SELECT * FROM c WHERE c.id = {id}")]
        //    IEnumerable<Food> foods,
        //    CosmosClient cosmosClient,
        //    string id)
        //{
        //    if (foods == null || !foods.Any())
        //    {
        //        _logger.LogError($"Food {id} not found. Delete operation impossible");
        //        return new NotFoundResult();
        //    }

        //    var food = foods.First();

        //    Container container = cosmosClient.GetContainer(id, "%MyContainer%");          
        //    await container.DeleteItemAsync<Food>(id, new PartitionKey(food.Name));

        //    return new OkResult();
        //}
    }
}
