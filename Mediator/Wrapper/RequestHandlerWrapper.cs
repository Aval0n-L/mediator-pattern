using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Data;
using Mediator.Services.Interfaces;
using Mediator.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator.Wrapper;

/// <summary>
/// Wrapper for requests that don't return a result//commands
/// Enables unified access through object-based dispatching
/// </summary>
public abstract class RequestHandlerWrapper : RequestHandlerBaseWrapper
{
    public abstract Task<Unit> HandleAsync(IRequest request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

public sealed class InternalRequestHandlerWrapper<TRequest> : RequestHandlerWrapper
    where TRequest : IRequest
{
    public override async Task<object?> HandleAsync(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        return await HandleAsync((IRequest) request, serviceProvider, cancellationToken).ConfigureAwait(false);
    }

    public override Task<Unit> HandleAsync(IRequest request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        // Create handler delegate
        async Task<Unit> HandleAsync(CancellationToken ct = default)
        {
            var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();
            await handler.HandleAsync((TRequest) request, ct = default ? cancellationToken : ct);
            return Unit.Value;
        };

        // Get behaviors
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TRequest, Unit>>().ToArray();

        // Build pipeline via iteration
        RequestHandlerDelegate<Unit> pipeline = HandleAsync;
        foreach (var behavior in behaviors.Reverse())
        {
            var next = pipeline;
            pipeline = (ct) => behavior.HandleAsync((TRequest)request, next, ct == default ? cancellationToken : ct);
        }

        return pipeline();
    }
}