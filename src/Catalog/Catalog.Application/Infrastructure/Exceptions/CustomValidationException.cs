using FluentValidation.Results;

namespace Catalog.Application.Infrastructure.Exceptions
{
    public class CustomValidationException : Exception, ICustomExceptionMarker
    {
        public CustomValidationException()
            : base("Ошибка валидации")
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
