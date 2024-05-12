using EECEBOT.API;
using EECEBOT.Application;
using EECEBOT.Infrastructure;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


if (!builder.Environment.IsDevelopment())
    builder.Services.AddAzureVaultConfiguration(builder.Configuration);

builder.Services
       .AddPresentation(builder.Host, builder.Configuration, builder.Environment)
       .AddApplication(builder.Configuration, builder.Environment.IsDevelopment())
       .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new [] {new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
    {
        RequireSsl = false,
        SslRedirect = false,
        LoginCaseSensitive = true,
        Users = new []
        {
            new BasicAuthAuthorizationUser
            {
                Login = app.Configuration["HangfireLogin"],
                PasswordClear = app.Configuration["HangfirePassword"]
            }
        }
    })}
});

app.UseSerilogRequestLogging();

if (!app.Environment.IsProduction())
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