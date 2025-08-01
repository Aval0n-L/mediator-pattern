using System.Collections.Generic;
using System.Reflection;

namespace Mediator.DI;

public class MediatorServiceConfiguration
{
    public List<Assembly> ListOfAssembly { get; } = new();

    public MediatorServiceConfiguration RegisterServicesFromAssemblies(params Assembly[] assemblies)
    {
        ListOfAssembly.AddRange(assemblies);

        return this;
    }
    public MediatorServiceConfiguration RegisterServicesFromAssemblies(Assembly assembly)
    {
        ListOfAssembly.Add(assembly);

        return this;
    }
}
