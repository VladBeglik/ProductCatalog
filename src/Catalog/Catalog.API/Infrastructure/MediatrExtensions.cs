using System.Diagnostics;
using System.Reflection;
using BookStore.App.Infrastructure;
using MediatR;
using MediatR.Pipeline;

namespace Catalog.API.Infrastructure
{
    public static class MediatrExtensions
    {
        public static IServiceCollection AddCustomMediatr(this IServiceCollection services, Assembly applicationAssemply)
        {
            services.AddMediatR(applicationAssemply);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));

            return services;
        }
    }

    #region IDontLogMediatrPayload
    public interface IDontLogMediatrPayload
    {

    }
    #endregion

    #region RequestPerformanceBehaviour
    public class RequestPerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;

        public RequestPerformanceBehaviour(ILogger<TRequest> logger)
        {
            _timer = new Stopwatch();
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            if (_timer.ElapsedMilliseconds > 400)
            {
                var name = typeof(TRequest).Name;
                
                if (request is IDontLogMediatrPayload)
                {
                    _logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} ms)",
                        name, _timer.ElapsedMilliseconds);
                }
                else
                {
                    _logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} ms) {@Request}",
                        name, _timer.ElapsedMilliseconds, request);
                }
            }

            return response;
        }
    }
    #endregion

    #region RequestLogger
    public class RequestLogger<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
    {
        private readonly ILogger _logger;

        public RequestLogger(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var name = typeof(TRequest).Name;

            if (request is IDontLogMediatrPayload)
            {
                _logger.LogInformation("Request: {Name}", name);
            }
            else
            {
                _logger.LogInformation("Request: {Name} {@Request}", name, request);
            }

            return Task.CompletedTask;
        }
    }
    #endregion

}
