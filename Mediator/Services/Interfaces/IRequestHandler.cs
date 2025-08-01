using System.Threading;
using System.Threading.Tasks;
using Mediator.Data;

namespace Mediator.Services.Interfaces;

/// <summary>
/// Handler for requests that don't return a response (commands)
/// </summary>
public interface IRequestHandler<in TRequest> where TRequest : IRequest
{
    Task HandleAsync(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Handler for requests that return a response/queries
/// </summary>
public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}