using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolERP.Api.Authorization;
using SchoolERP.Api.Filters;
using SchoolERP.Api.Middleware;
using SchoolERP.Api.Services;
using SchoolERP.Api.Swagger;
using SchoolERP.Application;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Infrastructure;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Persistence.Seed;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
#region Services

// Database
builder.Services.AddDbContext<SchoolERPDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Application Layer
builder.Services.AddApplication();

// Infrastructure Layer
builder.Services.AddInfrastructure(builder.Configuration);
// Current User (reads the authenticated user's identity from HttpContext for the Application/Infrastructure layers)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Controllers (with global FluentValidation action filter)
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
// Cors
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173", // React (Vite)
                "http://localhost:3000"  // React (CRA)
            )
            .AllowAnyHeader()
            .AllowAnyMethod();

        // Development
        // .AllowCredentials();
        // Cookie/Auth 
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureOptions<SwaggerJwtConfiguration>();
builder.Services.AddSwaggerGen();


#endregion

#region JWT Authentication

var jwt = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"]!)
        ),

        ClockSkew = TimeSpan.Zero
    };

    // JWT Middleware customization: return a consistent JSON body instead of an
    // empty 401/403 response when a token is missing, invalid, expired, or the
    // authenticated user lacks the required role/permission.
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "You are not authenticated, or your access token is missing/invalid/expired."
            });
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "You do not have permission to perform this action."
            });
        }
    };
});

// Permission-based authorization: PermissionPolicyProvider dynamically builds an
// AuthorizationPolicy for every [PermissionAuthorize] usage, evaluated by PermissionHandler.
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddAuthorization();

#endregion

var app = builder.Build();

#region Seed Data

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
    await DataSeeder.SeedAsync(app.Services);
}

#endregion

#region Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();