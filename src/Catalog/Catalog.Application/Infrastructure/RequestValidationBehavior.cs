using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace BookStore.App.Infrastructure
{
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count > 0)
            {
                throw new CustomValidationException(failures);
            }

            return next();
        }

        public class CustomValidationException : Exception
        {
            public CustomValidationException()
                : base((string)"Ошибка валидации")
            {
                Failures = new Dictionary<string, string[]>();
            }

            public CustomValidationException(IReadOnlyCollection<ValidationFailure> failures)
                : this()
            {
                var propertyNames = failures
                    .Select(e => e.PropertyName)
                    .Distinct();

                foreach (var propertyName in propertyNames)
                {
                    var propertyFailures = failures
                        .Where(e => e.PropertyName == propertyName)
                        .Select(e => e.ErrorMessage)
                        .ToArray();

                    Failures.Add(propertyName, propertyFailures);
                }
            }

            public IDictionary<string, string[]> Failures { get; }
        }
    }
}
