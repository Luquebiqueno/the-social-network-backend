using System.Reflection;
using TheSocialNetwork.Api;
using TheSocialNetwork.Api.Extensions;
using TheSocialNetwork.Application;
using TheSocialNetwork.Infra.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddPersistence(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

await app.RunAsync();

// REMARK: Required for functional and integration tests to work.
namespace TheSocialNetwork.Api
{
    public partial class Program;
}
