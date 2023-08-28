using System.Reflection;
using Automapper.Infrastructure.Infrastructure;

namespace Catalog.Application.Infrastructure.Mapping;

public class AutoMapperProfile : BaseAutoMapperProfile
{
    protected override Assembly RootAssembly => Assembly.GetExecutingAssembly();
}