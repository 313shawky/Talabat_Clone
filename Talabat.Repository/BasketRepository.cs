using Microsoft.EntityFrameworkCore.Query.Internal;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        public BasketRepository(IConnectionMultiplexer redis) // Ask CLR for object from class implement IConnectionMultiplexer
        {
            _database = redis.GetDatabase();
        }

        public async Task<bool> DeleteBasketAsync(string BasketId)
            => await _database.KeyDeleteAsync(BasketId);

        public async Task<CustomerBasket?> GetBasketAsync(string BasketId)
        {
            var basket = await _database.StringGetAsync(BasketId);
            //if (basket.IsNull)
            //    return null;
            //else
            //    return JsonSerializer.Deserialize<CustomerBasket>(basket);
            return basket.IsNull ? null : JsonSerializer.Deserialize<CustomerBasket>(basket);
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket Basket)
        {
            var JsonBasket = JsonSerializer.Serialize(Basket);
            var CreatedOrUpdated = await _database.StringSetAsync(Basket.Id, JsonBasket, TimeSpan.FromDays(1));
            if (!CreatedOrUpdated)
                return null;
            else
                return await GetBasketAsync(Basket.Id);
        }
    }
}
