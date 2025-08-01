using System.Threading;
using System.Threading.Tasks;
using Mediator.Data;

namespace Mediator;

public interface IMediator
{
    /// <summary>
    /// Dispatches an object-based request through the appropriate handler pipeline
    /// Used for dynamic dispatching when the exact type is not know at compile time
    /// </summary>
    Task<object> SendAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request/command that does not return a response/result
    /// </summary>
    Task SendAsync<TRequest>(IRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;

    /// <summary>
    /// Sends a request and return a response from the matching handler
    /// </summary>
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}