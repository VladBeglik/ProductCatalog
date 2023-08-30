using System.Reflection;
using Automapper.Infrastructure.Infrastructure;
using Identity.API.Infrastructure;
using Identity.API.Infrastructure.Filters;
using NodaTime;

var builder = WebApplication.CreateBuilder(args);
var appAssembly = typeof(AutoMapperProfile).GetTypeInfo().Assembly;
var pathBase = builder.Configuration["PATH_BASE"];

builder.Services

    
    .AddProjectDbContexts(builder.Configuration)
    .AddCustomSwagger(builder.Configuration)
    .AddAutoMapper(appAssembly)
    .AddRouting(o => { o.LowercaseUrls = true; })
    .AddSingleton<IClock>(sp => SystemClock.Instance)
    .AddHttpContextAccessor()
    .AddCustomCors()
    .AddCustomValidation(appAssembly)
    .AddCustomMediatr(appAssembly)
    .AddCustomIdentity(builder.Configuration, builder.Environment);


builder.Services
    .AddControllers(o =>
    {

        o.Filters.Add<ModelBindingErrorFilter>();
        o.Filters.Add<OperationCancelledExceptionFilter>();
        o.Filters.Add<CustomExceptionFilter>();
    })
    .AddCustomJsonOptions(builder.Environment);;




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
        o.SwaggerEndpoint($"{pathBase}/swagger/v1/swagger.json", $"{nameof(Identity)} API V1");
        //o.OAuthClientId("ro-1");

    }).UseCors(CorsPolicy.DEFAULT);
}

app.UseStaticFiles();
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();

app.UseCors();

app
    .MapControllers()
    .RequireCors(CorsPolicy.DEFAULT);

app.Run();