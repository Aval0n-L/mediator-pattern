using System.Threading;
using System.Threading.Tasks;
using Mediator.Data;
using Mediator.Services.Interfaces;

namespace Mediator.Console.Handlers;

public abstract class BaseCommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}