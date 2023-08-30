using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace Identity.API.Infrastructure;

public static class FluentValidationExtensions
{
    public static IServiceCollection AddCustomValidation(this IServiceCollection services, Assembly assembly)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}