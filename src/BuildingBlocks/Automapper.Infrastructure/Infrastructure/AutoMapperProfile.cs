using System.Reflection;

namespace Automapper.Infrastructure.Infrastructure;

public class AutoMapperProfile : BaseAutoMapperProfile
{
    protected override Assembly RootAssembly => Assembly.GetExecutingAssembly();
}