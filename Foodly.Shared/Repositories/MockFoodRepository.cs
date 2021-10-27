using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foodly.Shared.Repositories
{
    public class MockFoodRepository : IFoodRepository
    {
        private List<Food> foods = new List<Food>
        {
            new Food
            {
                Id = Guid.NewGuid(),
                Name = "Food1",
                MaxQuantity = 5,
                ActualQuantity = 2,
                OverQuantity = 0,
                OptionalQuantity = 1
            },
            new Food
            {
                Id = Guid.NewGuid(),
                Name = "Food2",
                MaxQuantity = 3,
                ActualQuantity = 5,
                OverQuantity = 1,
                OptionalQuantity = 1
            },
        };

        public Task<Food> AddFoodAsync(Food food)
        {
            foods.Add(food);
            return Task.FromResult(food);
        }

        public Task DeleteFood(Guid id)
        {
            foods.RemoveAt(foods.FindIndex(f => f.Id == id));
            return null;
        }

        public Task<Food[]> GetAllFoodsAsync()
        {
            return Task.FromResult(foods.ToArray());
        }

        public Task<Food> UpdateFoodAsync(Food food)
        {
            foods[foods.FindIndex(f => f.Id == food.Id)] = food;
            return Task.FromResult(food);
        }
    }
}
