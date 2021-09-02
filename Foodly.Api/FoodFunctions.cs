using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Foodly.Shared;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Linq;

namespace Foodly.Api
{
    public static class FoodFunctions
    {
        [FunctionName("GetAllFoods")]
        public static IActionResult GetAllFoods(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString",
                SqlQuery = "SELECT * FROM c")]
                IEnumerable<Food> foodList, ILogger log)
        {
            return new OkObjectResult(foodList);
        }


        [FunctionName("AddFood")]
        public async static Task<IActionResult> AddFood(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString")]IAsyncCollector<Food> foodOut,
            ILogger log)
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
                return new CreatedResult(UriFactory.CreateDocumentUri(databaseName, containerName, data.Id.ToString()), data);
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("UpdateFood")]
        public async static Task<IActionResult> UpdateFood(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateFood/{id}")]
            HttpRequest req,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString",
                Id = "{id}")]IAsyncCollector<Food> foodOut,
            ILogger log)
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

        [FunctionName("DeleteFood")]
        public static async Task<IActionResult> DeleteFood(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteFood/{id}")]
            HttpRequest request,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString",
                SqlQuery = "SELECT * FROM c WHERE c.id = {id}")]
            IEnumerable<Food> foods,
            [CosmosDB(
                databaseName: "%MyDatabase%",
                collectionName: "%MyContainer%",
                ConnectionStringSetting = "CosmosDBConnectionString")]
            DocumentClient documentClient,
            string id,
            ILogger log)
        {
            if (foods == null || !foods.Any())
            {
                return new NotFoundResult();
            }

            var food = foods.First();
            var uri = UriFactory.CreateDocumentUri("%MyDatabase%", "%MyContainer%", id);
            var options = new RequestOptions
            {
                PartitionKey = new PartitionKey(food.Name)
            };

            await documentClient.DeleteDocumentAsync(uri, options);

            return new OkResult();
        }
    }
}
