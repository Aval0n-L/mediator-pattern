using System;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Console.Commands;
using Mediator.Services.Interfaces;

namespace Mediator.Console.Handlers;

// Example handler for CreateUserCommand
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
{
    public async Task HandleAsync(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        // Simulate async operation (e.g., database insert)
        await Task.Delay(200, cancellationToken);
        
        // Log or perform the user creation logic
        System.Console.WriteLine($"Creating user: {request.Name} ({request.Email})");
    }
} 