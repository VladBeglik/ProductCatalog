using System.Reflection;
using BookStore.API.Infrastructure.Filters;
using Catalog.API.Infrastructure;
using Catalog.API.Infrastructure.Filters;
using Catalog.Application.Infrastructure;
using Catalog.Application.Infrastructure.Mapping;
using Microsoft.IdentityModel.Logging;
using NodaTime;

var builder = WebApplication.CreateBuilder(args);
var appAssembly = typeof(AutoMapperProfile).GetTypeInfo().Assembly;
var pathBase = builder.Configuration["PATH_BASE"];
IdentityModelEventSource.ShowPII = true;
builder.Services
    .AddCustomAuth(builder.Configuration)
    .AddProjectDbContexts(builder.Configuration)
    .AddCustomSwagger(builder.Configuration)
    .AddAutoMapper(appAssembly)
    .AddCustomHttpClients(builder.Configuration)
    .AddRouting(o => { o.LowercaseUrls = true; })
    .AddSingleton<IClock>(sp => SystemClock.Instance)
    .AddHttpContextAccessor()
    .AddCustomCors()
    .AddCustomValidation(appAssembly)
    .AddCustomMediatr(appAssembly);


builder.Services
    .AddControllers(o =>
    {

        o.Filters.Add<ModelBindingErrorFilter>();
        o.Filters.Add<OperationCancelledExceptionFilter>();
        o.Filters.Add<CustomExceptionFilter>();
    })
    .AddCustomJsonOptions(builder.Environment);;

builder.Services
    .AddScoped<ICurrentUserService, CurrentUserService>()
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
    ;


var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(app.DatabaseMigrate);

app.UseForwardedHeaders();

if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint($"{pathBase}/swagger/v1/swagger.json", $"{nameof(BookStore)} API V1");
        //o.OAuthClientId("ro-1");

    }).UseCors(CorsPolicy.DEFAULT);
}


// есть Middleware и Filter
//app.UseMiddleware<GlobalExceptionHandlingMiddleware>();


app.UseStaticFiles();
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthorization();
app.UseCors();

app
    .MapControllers()
    .RequireCors(CorsPolicy.DEFAULT);

app.Run();