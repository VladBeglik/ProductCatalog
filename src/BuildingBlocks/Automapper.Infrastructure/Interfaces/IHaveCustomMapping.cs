using AutoMapper;

namespace Automapper.Infrastructure.Interfaces;

public interface IHaveCustomMapping
{
    void CreateMappings(Profile configuration);
}