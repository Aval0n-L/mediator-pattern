using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Console.Entities;
using Microsoft.Extensions.Logging;

namespace Mediator.Console.Pipelines;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : GetUserQuery
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestType = request.GetType();
        _logger.LogDebug($"[{DateTime.Now:HH:mm:ss}] Starting {requestType}");
        try
        {
            var logMessage = @$"UserId: {request.UserId}.";

            _logger.LogDebug(@$"[Mediator] Starting with request: {requestType.Name}; {logMessage}; in {stopwatch.ElapsedMilliseconds}ms");

            var response = await next().ConfigureAwait(false);
            stopwatch.Stop();

            _logger.LogDebug(@$"[Mediator] Finished with request: {requestType.Name} and response: {response}; {logMessage}");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Mediator] Failed {requestType.Name} in {stopwatch.ElapsedMilliseconds}ms; Error message: {ex.Message}");
            throw;
        }
    }
}
