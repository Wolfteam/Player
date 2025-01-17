using Microsoft.AspNetCore.Identity;
using Player.Application;
using Player.Domain.Entities;
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
    }).AddBearerToken().AddIdentityCookies();
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
    builder.Services.AddEndpointsApiExplorer()
        .AddHealthChecks();
    builder.Services.AddSwaggerGen();

    builder.Services
        .AddPersistence(connectionString!)
        .AddValidation();

    builder.Services.AddUserService();

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