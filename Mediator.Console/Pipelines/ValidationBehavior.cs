using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Console.Entities;
using Microsoft.Extensions.Logging;

namespace Mediator.Console.Pipelines;


public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : GetUserQuery
{
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(request);

        if (Validator.TryValidateObject(request, context, results, true))
        {
            return await next().ConfigureAwait(false);
        }

        var errorMessage = GetValidationResultErrorMessage(results);
        _logger.LogError($"Validation for request failed: {errorMessage}");
        throw new ValidationException();
    }

    private static string GetValidationResultErrorMessage(IEnumerable<ValidationResult> validationResults)
    {
        return string.Join("; ", validationResults
            .Where(result => !string.IsNullOrEmpty(result.ErrorMessage))
            .Select(result => result.ErrorMessage));
    }
}
