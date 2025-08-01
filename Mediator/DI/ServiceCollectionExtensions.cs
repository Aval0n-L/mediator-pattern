using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mediator.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mediator.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services, Action<MediatorServiceConfiguration> configuration)
    {
        services.TryAddSingleton<IMediator, Mediator>();

        var serviceConfig = new MediatorServiceConfiguration();
        configuration.Invoke(serviceConfig);

        return services.RegisterMediatorServices(serviceConfig);
    }

    public static IServiceCollection RegisterMediatorServices(this IServiceCollection services, MediatorServiceConfiguration configuration)
    {
        if (!configuration.ListOfAssembly.Any())
        {
            throw new ArgumentException("No assemblies registered for mediator services.");
        }

        var assemblies = configuration.ListOfAssembly.Distinct().ToArray();

        RegisterImplementationToTypes(typeof(IRequestHandler<>), services, assemblies);
        RegisterImplementationToTypes(typeof(IRequestHandler<,>), services, assemblies);
        RegisterImplementationToTypes(typeof(IPipelineBehavior<,>), services, assemblies);

        return services;
    }

    private static void RegisterImplementationToTypes(Type originalType, IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var concretions = new List<Type>();
        var interfaces = new List<Type>();

        var types = assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsInterface && !t.ContainsGenericParameters)
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == originalType))
            .Where(t => t.FindInterfacesThatClose(originalType).Any())
            .ToList();

        foreach (var type in types)
        {
            var interfaceTypes = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == originalType)
                .ToArray();

            concretions.Add(type);
            foreach (var interfaceType in interfaceTypes)
            {
                interfaces.Fill(interfaceType);
            }
        }

        foreach (var iface in interfaces)
        {
            var matches = concretions.Where(c => iface.IsAssignableFrom(c));
            foreach (var match in matches)
            {
                services.TryAddEnumerable(ServiceDescriptor.Transient(iface, match));
                System.Console.WriteLine($"Registered: {iface} => {match}");//TODO REMOVE
            }
        }
    }

    private static void Fill<T>(this List<T> list, T value)
    {
        if (!list.Contains(value)) list.Add(value);
    }

    private static IEnumerable<Type> FindInterfacesThatClose(this Type type, Type openGenericType) 
        => FindInterfacesThatCloseCore(type, openGenericType).Distinct();

    private static IEnumerable<Type> FindInterfacesThatCloseCore(Type type, Type openGenericType)
    {
        if (type == null || type.IsAbstract || type.IsInterface) yield break;

        if(openGenericType.IsInterface)
        {
            foreach(var interfaceType in type.GetInterfaces().Where(i => i.IsGenericType && (i.GetGenericTypeDefinition() == openGenericType)))
            {
                yield return interfaceType;
            }
        }
        else if (type.BaseType!.IsGenericType && (type.BaseType!.GetGenericTypeDefinition() == openGenericType))
        {
            yield return type.BaseType!;
        }

        if(type.BaseType == typeof(object)) yield break;

        foreach(var inteerfaceType in FindInterfacesThatCloseCore(type.BaseType!, openGenericType))
        {
            yield return inteerfaceType;
        }
    }
}
