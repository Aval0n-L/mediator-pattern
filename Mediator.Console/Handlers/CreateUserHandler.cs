using System.Threading;
using System.Threading.Tasks;
using Mediator.Console.Commands;
using Mediator.Console.Entities;
using Microsoft.Extensions.Logging;

namespace Mediator.Console.Handlers;

public class CreateUserHandler : BaseCommandHandler<CreateUserRequest, UserResponse>
{
    private readonly ILogger<CreateUserHandler> _logger;

    public CreateUserHandler(ILogger<CreateUserHandler> logger)
    {
        _logger = logger;
    }

    public override async Task<UserResponse> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Created user: {request.Name}");
        await Task.Delay(100, cancellationToken);
        return new UserResponse
        {
            Id = request.UserId,
            Name = $"User {request.UserId}",
            Email = $"user{request.UserId}@example.com"
        };
    }
}