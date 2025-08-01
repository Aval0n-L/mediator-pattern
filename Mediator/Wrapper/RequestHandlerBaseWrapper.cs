using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mediator.Wrapper;

/// <summary>
/// Base abstract wrapper for dispatching request execution in a generic way
/// Used for caching compiled handler logic
/// </summary>
public abstract class RequestHandlerBaseWrapper
{
    public abstract Task<object> HandleAsync(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}