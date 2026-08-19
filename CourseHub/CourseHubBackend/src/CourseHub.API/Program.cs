using CourseHub.API.Extensions;
using CourseHub.API.Middleware;
using CourseHub.API.Security;
using CourseHub.Application;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Common.Options;
using CourseHub.Infrastructure;
using CourseHub.Infrastructure.Email;
using CourseHub.Infrastructure.Persistence.Context;
using CourseHub.Infrastructure.Persistence.Seed;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    // Interim domain-exception -> HTTP status mapping until Phase 10's
    // full ProblemDetails-based global exception handling replaces it.
    options.Filters.Add<DomainExceptionFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwtSupport();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddApiAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// No real email provider is configured yet (see IEmailSender). In
// Development, emails are logged instead of sent so the password-reset
// flow is testable locally. Everywhere else, a missing provider fails
// loudly instead of silently pretending to work - see NotConfiguredEmailSender.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailSender, DevelopmentEmailSender>();
}
else
{
    builder.Services.AddScoped<IEmailSender, NotConfiguredEmailSender>();
}

var app = builder.Build();

// Idempotent startup seeding: guarantees the default system roles
// (SuperAdmin/Admin/Teacher/Student) and the single Institution
// landing-page profile exist. Safe to run on every startup.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CourseHubDbContext>();
    var seedOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value;
    await DatabaseSeeder.SeedAsync(dbContext, seedOptions);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Exposed for WebApplicationFactory<Program> in integration tests.
public partial class Program
{
}
