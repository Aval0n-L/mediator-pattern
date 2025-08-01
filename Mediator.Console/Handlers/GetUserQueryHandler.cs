using System.Threading;
using System.Threading.Tasks;
using Mediator.Console.Entities;
using Mediator.Services.Interfaces;

namespace Mediator.Console.Handlers;

// Example handler for GetUserQuery
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserResponse>
{
    public async Task<UserResponse> HandleAsync(GetUserQuery request, CancellationToken cancellationToken = default)
    {
        // Simulate async operation (e.g., database call)
        await Task.Delay(100, cancellationToken);
        
        // Return mock user data
        return new UserResponse
        {
            Id = request.UserId,
            Name = $"User {request.UserId}",
            Email = $"user{request.UserId}@example.com"
        };
    }
} 