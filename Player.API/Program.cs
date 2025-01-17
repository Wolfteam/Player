using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Player.API.Authorization;
using Player.API.Middleware;
using Player.API.Models;
using Player.Application;
using Player.Domain.Entities;
using Player.Domain.Interfaces;
using Player.Domain.Utils;
using Player.Infrastructure.Persistence;
using Serilog;
using Serilog.Formatting.Json;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Debug()
    .WriteTo.Console(new JsonFormatter()).CreateBootstrapLogger();

try
{
    Log.Information("Creating builder...");
    var builder = WebApplication.CreateBuilder(args);

    string? connectionString = builder.Configuration.GetConnectionString("Default");
    Check.NotEmpty(connectionString, nameof(connectionString));

    Log.Information("Configuring services...");
    builder.Logging.AddSerilog();
    builder.Services.AddMvc();
#if DEBUG
    builder.Services.AddControllersWithViews()
        .AddRazorRuntimeCompilation();
#endif
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddAuthentication(o =>
    {
        o.DefaultScheme = IdentityConstants.ApplicationScheme;
        o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    }).AddBearerToken(o =>
    {
        o.ClaimsIssuer = "App";
    }).AddIdentityCookies();
    builder.Services.ConfigureApplicationCookie(o =>
    {
        o.ExpireTimeSpan = TimeSpan.FromMinutes(15);
        o.LogoutPath = "/Account/Logout";
    });

    builder.Services.Configure<IdentityOptions>(o =>
    {
        o.User.RequireUniqueEmail = true;
        o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        o.Lockout.MaxFailedAccessAttempts = 3;
        o.Password.RequireDigit = true;
        o.Password.RequiredLength = 8;
        o.Password.RequireUppercase = false;
        o.Password.RequireLowercase = false;
        o.Password.RequireNonAlphanumeric = false;
    });

    builder.Services.AddIdentityCore<User>()
        .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<User>>()
        .AddDefaultTokenProviders()
        .AddSignInManager();
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version")
        );
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
    builder.Services.AddEndpointsApiExplorer()
        .AddHealthChecks();
    builder.Services.AddSwaggerGen(c =>
    {
        string xmlPath = Path.Combine(AppContext.BaseDirectory, "API.xml");
        c.IncludeXmlComments(xmlPath);

        var securityScheme = new OpenApiSecurityScheme
        {
            Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
            In = ParameterLocation.Header,
            Name = HeaderNames.Authorization,
            Type = SecuritySchemeType.ApiKey
        };

        string scheme = BearerTokenDefaults.AuthenticationScheme;
        var securityReq = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = scheme
                    }
                },
                Array.Empty<string>()
            }
        };
        c.AddSecurityDefinition(scheme, securityScheme);
        c.AddSecurityRequirement(securityReq);
    });


    builder.Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
    builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
    builder.Services.AddTransient<ICurrentLoggedUser, CurrentLoggedUser>();
    builder.Services
        .AddPersistence(connectionString!)
        .AddValidation();

    builder.Services
        .AddUserService()
        .AddPlaylistService()
        .AddMediaService();

    Log.Information("Building app...");
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.Services.ApplyDatabaseMigrations();

    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors(options =>
        options.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
    app.UseHealthChecks("/healthcheck");
    app.UseMiddleware<ExceptionHandlerMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapDefaultControllerRoute();

    Log.Information("Running app...");
    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}