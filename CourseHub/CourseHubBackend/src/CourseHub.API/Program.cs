using CourseHub.API.Extensions;
using CourseHub.API.HealthChecks;
using CourseHub.API.Middleware;
using CourseHub.API.Security;
using CourseHub.Application;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Common.Options;
using CourseHub.Infrastructure;
using CourseHub.Infrastructure.Email;
using CourseHub.Infrastructure.Persistence.Context;
using CourseHub.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwtSupport();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddApiAuthorization();

// Phase 10: global exception handling. Every unhandled exception from
// any middleware/controller/Application/Infrastructure code is caught
// exactly once by GlobalExceptionHandler and turned into a consistent
// RFC 7807 ProblemDetails JSON response — see that class for the
// exception -> status code mapping.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(options =>
{
    // Applies to ProblemDetails produced anywhere (GlobalExceptionHandler,
    // [ApiController]'s automatic 400 responses, ValidationProblem(), and
    // status-code pages) so every error response — not just unhandled
    // exceptions — carries the same traceId for log correlation.
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
    };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Phase 16: Docker/Cloud-readiness. Split into two endpoints because
// they answer different questions and Kubernetes/Docker Compose/load
// balancers treat them differently:
// - /health/live: "is the process up and responding at all" — no
//   dependency checks. A failing DB should NOT restart the container
//   (that wouldn't fix a DB outage and would just cause a restart loop).
// - /health/ready: "is this instance ready to serve real traffic" —
//   includes the database check below. Used to gate load-balancer
//   traffic / Compose's `depends_on: condition: service_healthy`.
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready" });

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
// (SuperAdmin/Admin/Teacher/Student), the global permission catalog, and
// the single Institution landing-page profile exist. Safe to run on
// every startup.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CourseHubDbContext>();

    // Applying migrations here (rather than requiring a separate manual
    // `dotnet ef database update` step) is what makes `docker compose up`
    // fully self-sufficient — the API container brings its own schema up
    // to date against a fresh Postgres container with no extra tooling
    // needed inside the runtime image (it only has the ASP.NET Core
    // runtime, not the SDK or the dotnet-ef CLI). Safe to run on every
    // startup: EF Core tracks which migrations have already been applied
    // and this is a no-op once the schema is current. For local dev
    // outside Docker this also means the manual migration step described
    // in "Local setup" below is now optional, not required.
    await dbContext.Database.MigrateAsync();

    var seedOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value;
    await DatabaseSeeder.SeedAsync(dbContext, seedOptions);
}

// Configure the HTTP request pipeline.

// Must be one of the first middleware registered so it wraps every other
// middleware/controller downstream. UseStatusCodePages ensures routes
// that don't match a controller (e.g. a typo'd URL, a plain 404 with no
// thrown exception) also get a ProblemDetails-shaped body instead of an
// empty response, for a consistent error contract across the whole API.
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// See the AddHealthChecks() comment above for why these are split.
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
});

app.Run();

// Exposed for WebApplicationFactory<Program> in integration tests.
public partial class Program
{
}
