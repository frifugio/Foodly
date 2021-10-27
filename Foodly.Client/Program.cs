using Foodly.Shared.Repositories;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Foodly.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            var baseAddress = builder.Configuration["BaseAddress"] ?? builder.HostEnvironment.BaseAddress;
            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(baseAddress) });

            if (builder.HostEnvironment.IsDevelopment())
            {
                builder.Services.AddScoped<IFoodRepository, MockFoodRepository>();
            }
            else
            {
                builder.Services.AddScoped<IFoodRepository, FoodRepository>();
            }

            await builder.Build().RunAsync();
        }
    }
}
