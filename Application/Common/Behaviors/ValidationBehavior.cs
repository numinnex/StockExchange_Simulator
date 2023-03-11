using Application.Common.Intefaces;
using Application.Common.Models;
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest , TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult 
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResult = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResult.Where(x => x.Errors.Any())
            .SelectMany(r => r.Errors)
            .Select(x => new Error() { Message = x.ErrorMessage , Code = x.ErrorCode})
            .ToArray();

        if (failures.Any())
            return CreateValidationResult<TResponse>(failures);

        return await next();
    }

    private static TResult CreateValidationResult<TResult>(Error[] errors)
        where TResult : IResult
    {
        var objValidation = typeof(Result<>)
            .GetGenericTypeDefinition()
            .MakeGenericType(typeof(TResult).GenericTypeArguments[0])
            .GetMethod(nameof(Result<IDto>.Failure))!
            .Invoke(null, new object?[] { errors });

        return (TResult)objValidation!;
    }
}