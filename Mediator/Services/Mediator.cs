using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Data;
using Mediator.Extensions;
using Mediator.Wrapper;

namespace Mediator;

public sealed class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    // Cache compiled handlers for performance
    private static readonly ConcurrentDictionary<Type, RequestHandlerBaseWrapper> _requestHandlers = new();

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public Task<object> SendAsync(object request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request), "Request cannot be null.");

        var requestType = request.GetType();
        var wrapper = _requestHandlers.GetOrAdd(requestType, RequestHandlerWrapperFactory.CreatePolymorphicHandlerFactory());

        return wrapper.HandleAsync(request, _serviceProvider, cancellationToken);
    }

    public Task SendAsync<TRequest>(IRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        if (request == null) throw new ArgumentNullException(nameof(request), "Request cannot be null.");

        var requestType = request.GetType();
        var wrapper = (RequestHandlerWrapper)_requestHandlers.GetOrAdd(requestType, RequestHandlerWrapperFactory.CreateCommandHandlerFactory());

        return wrapper.HandleAsync(request, _serviceProvider, cancellationToken);
    }

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request), "Request cannot be null.");

        var requestType = request.GetType();
        var wrapper = (RequestHandlerResponseWrapper<TResponse>)_requestHandlers.GetOrAdd(requestType, RequestHandlerWrapperFactory.CreateResponseHandlerFactory<TResponse>());

        return wrapper.HandleAsync(request, _serviceProvider, cancellationToken);
    }
}
