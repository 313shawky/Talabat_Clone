using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services;

namespace Talabat.APIs.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _expireTimeInSeconds;

        public CachedAttribute(int ExpireTimeInSeconds)
        {
            _expireTimeInSeconds = ExpireTimeInSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var CacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var CacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var CachedResponse = await CacheService.GetCachedResponseAsync(CacheKey);
            if(!string.IsNullOrEmpty(CachedResponse))
            {
                var ContentResult = new ContentResult()
                {
                    Content = CachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = ContentResult;
                return;
            }
            var ExecutedEndPointContext = await next.Invoke(); // will execute endpoint
            if(ExecutedEndPointContext.Result is OkObjectResult result)
            {
                await CacheService.CacheResponseAsync(CacheKey, result.Value, TimeSpan.FromSeconds(_expireTimeInSeconds));
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var KeyBuilder = new StringBuilder();
            // sort = name
            // page index = 1
            // page size = 5
            KeyBuilder.Append(request.Path); // api/products
            foreach(var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                KeyBuilder.Append($"|{key}-{value}");
            }
            return KeyBuilder.ToString();
        }
    }
}
