using EECEBOT.API;
using EECEBOT.API.Common;
using EECEBOT.Application;
using EECEBOT.Infrastructure;
using Hangfire;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddAzureVaultConfiguration(builder.Configuration)
       .AddPresentation(builder.Host)
       .AddApplication(builder.Configuration)
       .AddInfrastructure(builder.Configuration);

var app = builder.Build();

ApplicationStartChecks.Check(app.Services, app.Configuration);

app.UseHangfireDashboard();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler("/error");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHangfireDashboard();

app.Run();