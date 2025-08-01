using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Data;
using Mediator.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator.Wrapper;

/// <summary>
/// Typed wrapper for requests that return a response
/// Enables dispatching of <see cref="IRequest{TResponse}"/> via DI-resolved handlers and behaviors
/// </summary>
public abstract class RequestHandlerResponseWrapper<TResponse> : RequestHandlerBaseWrapper
{
    public abstract Task<TResponse> HandleAsync(IRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

public sealed class InternalRequestHandlerResponseWrapper<TRequest, TResponse> : RequestHandlerResponseWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{
    public override async Task<object?> HandleAsync(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        return await HandleAsync((IRequest<TResponse>)request, serviceProvider, cancellationToken).ConfigureAwait(false);
    }

    public override async Task<TResponse> HandleAsync(
        IRequest<TResponse> request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        // Create handler delegate
        RequestHandlerDelegate<TResponse> handlerDelegate = async (ct) =>
        {
            var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
            return await handler.HandleAsync((TRequest)request, ct = default ? cancellationToken : ct);
        };

        // Get behaviors
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>().ToArray();

        // Build pipeline via iteration
        RequestHandlerDelegate<TResponse> pipeline = handlerDelegate;
        foreach (var behavior in behaviors.Reverse())
        {
            var next = pipeline;
            pipeline = (ct) => behavior.HandleAsync((TRequest)request, next, ct == default ? cancellationToken : ct);
        }

        return await pipeline();
    }
}
