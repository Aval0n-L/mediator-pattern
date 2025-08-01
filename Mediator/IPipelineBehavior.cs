using System.Threading;
using System.Threading.Tasks;

namespace Mediator;

/// <summary>
/// IPipelineBehavior interface defines a behavior that can be applied to requests and responses in the mediator pattern.
/// </summary>
public interface IPipelineBehavior<in TRequest, TResponse> where TRequest : notnull
{
    Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken = default);