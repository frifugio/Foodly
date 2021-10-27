using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace Foodly.Shared.Repositories
{
    public class FoodRepository : IFoodRepository
    {
        private readonly HttpClient _httpClient;

        public FoodRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Food> AddFoodAsync(Food newFood)
        {
             await _httpClient.PostAsJsonAsync<Food>($"/api/AddFood", newFood);
            return newFood;
        }

        public async Task DeleteFood(Guid id)
        {
            await _httpClient.DeleteAsync($"/api/DeleteFood/{id}");
        }

        public async Task<Food[]> GetAllFoodsAsync()
        {
            return await _httpClient.GetFromJsonAsync<Food[]>("/api/GetAllFoods");
        }

        public async Task<Food> UpdateFoodAsync(Food food)
        {
            _ = await _httpClient.PutAsJsonAsync<Food>($"/api/UpdateFood/{food.Id}", food);
            return food;
        }
    }
}
