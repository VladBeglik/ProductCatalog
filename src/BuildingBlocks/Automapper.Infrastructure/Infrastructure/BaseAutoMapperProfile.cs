using System.Reflection;
using AutoMapper;

namespace Automapper.Infrastructure.Infrastructure;

public abstract class BaseAutoMapperProfile : Profile
{
    protected BaseAutoMapperProfile()
    {
        LoadStandardMappings();
        LoadCustomMappings();
        LoadConverters();
    }

    protected abstract Assembly RootAssembly { get; }

    protected virtual void LoadConverters()
    {

    }

    private void LoadStandardMappings()
    {
        var mapsFrom = MapperProfileHelper.LoadStandardMappings(RootAssembly);

        foreach (var map in mapsFrom)
        {
            CreateMap(map.Source, map.Destination).ReverseMap();
        }
    }

    private void LoadCustomMappings()
    {
        var mapsFrom = MapperProfileHelper.LoadCustomMappings(RootAssembly);

        foreach (var map in mapsFrom)
        {
            map.CreateMappings(this);
        }
    }
}