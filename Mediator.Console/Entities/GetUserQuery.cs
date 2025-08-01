using System;
using Mediator.Data;

namespace Mediator.Console.Entities;

// Example query that returns a response
public class GetUserQuery : IRequest<UserResponse>
{
    public Guid UserId { get; set; }
}

public class UserResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
