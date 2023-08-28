using MicroElements.Swashbuckle.NodaTime;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Catalog.API.Infrastructure
{
    public static class SwaggerExtensions
    {
        #region class SwaggerDocumentFilter
        private class SwaggerDocumentFilter : IDocumentFilter
        {
            private readonly string _pathBase;

            public SwaggerDocumentFilter(string pathBase)
            {
                _pathBase = pathBase;
            }
            public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            {
                swaggerDoc.Servers.Add(new OpenApiServer() { Url = _pathBase });
            }
        }
        #endregion

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration config, string version = "v1")
        {
            services.AddSwaggerGen(o =>
            {
                o.CustomSchemaIds(type => type.ToString());

                var pathBase = config["PATH_BASE"];

                if (!string.IsNullOrEmpty(pathBase))
                {
                    o.DocumentFilter<SwaggerDocumentFilter>(pathBase);
                }

                o.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = $"{nameof(BookStore)} API",
                    Version = version,
                });
                
                o.ConfigureForNodaTimeWithSystemTextJson();
            });

            return services;
        }
    }
}
