using System;
using System.Threading.Tasks;

namespace Foodly.Shared.Repositories
{
    public interface IFoodRepository
    {
        public Task<Food[]> GetAllFoodsAsync(); 
        public Task<Food> AddFoodAsync(Food food); 
        public Task<Food> UpdateFoodAsync(Food food); 
        public Task DeleteFood(Guid id); 
    }
}
