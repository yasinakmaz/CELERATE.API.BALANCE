namespace CELERATE.API.API.Extensions
{
    public static class CorsExtensions
    {
        public static IApplicationBuilder UseOptimizedCors(this IApplicationBuilder app)
        {
            app.UseCors(builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("Content-Disposition")
                    .AllowCredentials()
                    .SetIsOriginAllowed(origin =>
                    {
                        return true;
                    });
            });

            return app;
        }
    }
}
