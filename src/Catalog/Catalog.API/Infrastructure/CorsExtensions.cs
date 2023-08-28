namespace Catalog.API.Infrastructure
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy.SWAGGER, policy =>
                     policy
                        .WithOrigins(
                            "http://localhost:5000",
                            "https://localhost:5001"
                         )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials())
                        ;

                options.AddPolicy(CorsPolicy.DEFAULT, policy =>
                     policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(_ => true)
                        .AllowCredentials())
                        ;
            });

            return services;
        }
    }

    public static class CorsPolicy
    {
        public const string SWAGGER = "swaggerCors";
        public const string DEFAULT = "defaultCors";
    }
}
