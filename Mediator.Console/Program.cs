using System;
using System.Reflection;
using System.Threading.Tasks;
using Mediator.Console.Commands;
using Mediator.Console.Entities;
using Mediator.Console.Pipelines;
using Mediator.DI;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator.Console;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // Set up dependency injection
        var services = new ServiceCollection();

        // Start Register the mediator
        services.AddLogging();
        services.AddMediator(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        services.AddScoped<IPipelineBehavior<GetUserQuery, UserResponse>, LoggingBehavior<GetUserQuery, UserResponse>>();
        services.AddScoped<IPipelineBehavior<GetUserQuery, UserResponse>, ValidationBehavior<GetUserQuery, UserResponse>>();
        // End Register the mediator


        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        System.Console.WriteLine("=== Mediator Pattern Demo ===");
        System.Console.WriteLine();

        try
        {
            // Example 1: Using a query (returns a response)
            System.Console.WriteLine("1. Executing GetUserQuery...");
            var getUserQuery = new GetUserQuery { UserId = Guid.NewGuid() };
            var userResponse = await mediator.SendAsync<UserResponse>(getUserQuery);
            System.Console.WriteLine($"   Retrieved user: {userResponse.Name} ({userResponse.Email})");
            System.Console.WriteLine();

            // Example 2: Using a command (no response)
            System.Console.WriteLine("2. Executing CreateUserCommand...");
            var createUserCommand_John = new CreateUserCommand
            {
                Name = "John Doe",
                Email = "john.doe@example.com"
            };
            await mediator.SendAsync<CreateUserCommand>(createUserCommand_John);
            System.Console.WriteLine("   Command executed successfully");
            System.Console.WriteLine();

            // Example 3: Using a command but when interfaces closes 
            System.Console.WriteLine("3. Executing CreateUserCommand...");
            var createUserCommand_Alice = new CreateUserRequest
            {
                Name = "Alice Mour",
                Email = "alice.mour@example.com"
            };
            var newUser = await mediator.SendAsync<UserResponse>(createUserCommand_Alice);
            System.Console.WriteLine($"   Retrieved user: {newUser.Name} ({createUserCommand_Alice.Email})");
            System.Console.WriteLine();

            System.Console.WriteLine("=== Demo completed successfully! ===");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error: {ex.Message}");
            System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }

        System.Console.WriteLine();
        System.Console.WriteLine("Press any key to exit...");
        System.Console.ReadKey();
    }
}