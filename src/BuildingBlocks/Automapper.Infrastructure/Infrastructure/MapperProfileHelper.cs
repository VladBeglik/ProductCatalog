using System.Reflection;
using Automapper.Infrastructure.Interfaces;

namespace Automapper.Infrastructure.Infrastructure;

public static class MapperProfileHelper
{
    public static IEnumerable<Map> LoadStandardMappings(Assembly rootAssembly)
    {
        var types = rootAssembly.GetExportedTypes();

        var mapsFrom = (
            from type in types
            from instance in type.GetInterfaces()
            where
                instance.IsGenericType && instance.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                !type.IsAbstract &&
                !type.IsInterface
            select new Map
            {
                Source = type.GetInterfaces().First().GetGenericArguments().First(),
                Destination = type
            }).ToList();

        return mapsFrom;
    }

    public static IEnumerable<IHaveCustomMapping> LoadCustomMappings(Assembly rootAssembly)
    {
        var types = rootAssembly.GetExportedTypes();

        var mapsFrom = (
            from type in types
            from instance in type.GetInterfaces()
            where
                typeof(IHaveCustomMapping).IsAssignableFrom(type) &&
                !type.IsAbstract &&
                !type.IsInterface
            select (IHaveCustomMapping)Activator.CreateInstance(type)).ToList();

        return mapsFrom;
    }
}