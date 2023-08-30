namespace Identity.API.Infrastructure;

public static class HttpClientExtensions
{
    public static IServiceCollection AddCustomHttpClients(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<AuthorizationDelegatingHandler>();

        services
            .AddHttpClient("catalog", cl => cl.BaseAddress = new Uri(config.GetConnectionString("Catalog")!))
            .AddHttpMessageHandler<AuthorizationDelegatingHandler>()
            .CustomConfigurePrimaryHttpMessageHandler()
            ;

        return services;
    }

    private static IHttpClientBuilder CustomConfigurePrimaryHttpMessageHandler(this IHttpClientBuilder builder)
    {
        builder
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            });

        return builder;
    }
}

public class AuthorizationDelegatingHandler : DelegatingHandler
{
    readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authorizationHeader = _httpContextAccessor.HttpContext?
            .Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authorizationHeader))
        {
            request.Headers.Add("Authorization", new List<string>() { authorizationHeader });
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
