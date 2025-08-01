using System;
using System.Linq;
using Mediator.Data;
using Mediator.Wrapper;

namespace Mediator.Extensions;

public static class RequestHandlerWrapperFactory
{
    /// <summary>
    /// A generic factory for <see cref="IRequest"/> with response
    /// </summary>
    public static Func<Type, RequestHandlerBaseWrapper> CreateResponseHandlerFactory<TResponse>()
    {
        return static requestType =>
        {
            var wrapperType = typeof(InternalRequestHandlerResponseWrapper<,>).MakeGenericType(requestType, typeof(TResponse));
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper for {requestType}.");
            return (RequestHandlerBaseWrapper)wrapper;
        };
    }

    /// <summary>
    /// Creates a factory for <see cref="IRequest"/> without response
    /// </summary>
    public static Func<Type, RequestHandlerBaseWrapper> CreateCommandHandlerFactory()
    {
        return static requestType =>
        {
            var wrapperType = typeof(InternalRequestHandlerWrapper<>).MakeGenericType(requestType);
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper for {requestType}.");
            return (RequestHandlerBaseWrapper)wrapper;
        };
    }

    /// <summary>
    /// A factory for polymorphic request, selects the type at runtime (universal option)
    /// </summary>
    public static Func<Type, RequestHandlerBaseWrapper> CreatePolymorphicHandlerFactory()
    {
        return static requestType =>
        {
            Type wrapperType;

            var requestInterfaceType = requestType
                .GetInterfaces()
                .FirstOrDefault(static x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));

            if (requestInterfaceType is not null)
            {
                var responseType = requestInterfaceType.GetGenericArguments()[0];
                wrapperType = typeof(InternalRequestHandlerResponseWrapper<,>).MakeGenericType(requestType, responseType);
            }
            else
            {
                requestInterfaceType = requestType
                    .GetInterfaces()
                    .FirstOrDefault(static x => x == typeof(IRequest));
                if (requestInterfaceType is null)
                {
                    throw new ArgumentException($"{requestType.Name} doesn't implement {nameof(IRequest)}");
                }

                wrapperType = typeof(InternalRequestHandlerWrapper<>).MakeGenericType(requestType);
            }

            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper for {requestType}.");
            return (RequestHandlerBaseWrapper)wrapper;
        };
    }
}
