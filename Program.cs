using System.Text;
using System.Text.Json.Serialization;
using Inventory_Management_dotnet.Data;
using Inventory_Management_dotnet.Exceptions;
using Inventory_Management_dotnet.Models.Entities;
using Inventory_Management_dotnet.Options;
using Inventory_Management_dotnet.Repositories;
using Inventory_Management_dotnet.Repositories.Interfaces;
using Inventory_Management_dotnet.Services;
using Inventory_Management_dotnet.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Serilog;

Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add repositories to the container.
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();

    // Add services to the container.
    builder.Services.AddScoped<IFileStorageService, FileStorageService>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ICookieService, CookieService>();

    builder.Services.AddOptions<BlobStorageOptions>()
        .BindConfiguration(BlobStorageOptions.SectionName)
        .ValidateDataAnnotations()
        .ValidateOnStart();
    builder.Services.AddOptions<JwtSettingsOptions>()
        .BindConfiguration(JwtSettingsOptions.SectionName)
        .ValidateDataAnnotations()
        .ValidateOnStart();
    builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(
        builder.Configuration.GetConnectionString("DbConnectionString")
    ));
    builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));
    builder.Services.AddControllers().AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter()
            );
        }
    );

    var jwtSettings = builder.Configuration
                        .GetSection(JwtSettingsOptions.SectionName)
                        .Get<JwtSettingsOptions>()
                        ?? throw new InvalidOperationException(
                            "JWT settings are not configured.");

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // keep claims names as defined, no overwriting
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ClockSkew = TimeSpan.Zero,
                NameClaimType = JwtRegisteredClaimNames.Name,
                RoleClaimType = "role"
            };

            // Redirect middleware to find access_token in the cookie
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    // get the access_token
                    if(context.Request.Cookies.TryGetValue("access_token", out var token))
                    {
                        context.Token = token;
                    }

                    return Task.CompletedTask;
                }
            };

        });

    builder.Services.AddAuthorization();
    builder.Services.AddProblemDetails();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Seed user roles before any auth endpoint hits the API
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        foreach(var role in new[] {Roles.Staff, Roles.Admin})
        {
            if(!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseExceptionHandler();

    app.UseHttpsRedirection();

    app.UseSerilogRequestLogging();

    
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.MapControllers();


    app.Run();

} catch(HostAbortedException)
{
    // EF Core tooling aborts the host after reading design-time services.
    Log.Information("EF core has read design time services");
} 
catch (Exception ex)
{
    Log.Fatal(ex, "Server terminated unexpectedly!");
    Console.WriteLine(ex.Message);
} finally
{
    Log.CloseAndFlush();
}