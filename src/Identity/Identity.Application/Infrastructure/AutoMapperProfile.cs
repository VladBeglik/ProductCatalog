using System.Reflection;
using Automapper.Infrastructure.Infrastructure;

namespace Identity.Application.Infrastructure;

public class AutoMapperProfile : BaseAutoMapperProfile
{
    protected override Assembly RootAssembly => Assembly.GetExecutingAssembly();
}