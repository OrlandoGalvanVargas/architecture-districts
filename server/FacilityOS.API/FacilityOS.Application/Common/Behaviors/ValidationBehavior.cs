using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FacilityOS.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validationTasks = new List<Task<ValidationResult>>();

        var rootValidatorType = typeof(IValidator<>).MakeGenericType(typeof(TRequest));
        var rootValidator = _serviceProvider.GetService(rootValidatorType) as IValidator;

        if (rootValidator != null)
        {
            var context = new ValidationContext<TRequest>(request);
            validationTasks.Add(rootValidator.ValidateAsync(context, cancellationToken));
        }

        foreach (var property in typeof(TRequest).GetProperties())
        {
            var propertyValue = property.GetValue(request);
            if (propertyValue == null) continue;

            var propertyValidatorType = typeof(IValidator<>).MakeGenericType(property.PropertyType);
            var propertyValidator = _serviceProvider.GetService(propertyValidatorType) as IValidator;

            if (propertyValidator != null)
            {
                var propertyContext = new ValidationContext<object>(propertyValue);
                validationTasks.Add(propertyValidator.ValidateAsync(propertyContext, cancellationToken));
            }
        }

        if (validationTasks.Any())
        {
            var validationResults = await Task.WhenAll(validationTasks);

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
