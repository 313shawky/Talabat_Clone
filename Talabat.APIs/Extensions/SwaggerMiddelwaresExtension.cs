namespace Talabat.APIs.Extensions
{
    public static class SwaggerMiddelwaresExtension
    {
        public static void UseSwaggerMiddlewares(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}
