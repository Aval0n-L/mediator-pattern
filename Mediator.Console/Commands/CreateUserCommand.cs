using System;
using Mediator.Data;

namespace Mediator.Console.Commands;

// Example command that doesn't return a response
public class CreateUserCommand : IRequest
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
