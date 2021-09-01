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

namespace Foodly.Api
{
    public static class FoodFunctions
    {
        [FunctionName("GetFoods")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var foods = new List<Food>()
            {
                new Food { Name= "Carne" },
                new Food { Name= "Pesce" },
            };

            return new OkObjectResult(foods);
        }
    }
}
