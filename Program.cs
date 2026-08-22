using System.Text.Json.Serialization;
using Inventory_Management_dotnet.Data;
using Inventory_Management_dotnet.Exceptions;
using Inventory_Management_dotnet.Options;
using Inventory_Management_dotnet.Repositories;
using Inventory_Management_dotnet.Repositories.Interfaces;
using Inventory_Management_dotnet.Services;
using Inventory_Management_dotnet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddScoped<IFileStorageService, FileStorageService>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddOptions<BlobStorageOptions>()
        .BindConfiguration(BlobStorageOptions.SectionName)
        .ValidateDataAnnotations()
        .ValidateOnStart();
    builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(
        builder.Configuration.GetConnectionString("DbConnectionString")
    ));
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

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseSerilogRequestLogging();

    app.UseExceptionHandler();

    app.MapControllers();

    app.Run();

} catch (Exception ex)
{
    Log.Fatal(ex, "Server terminated unexpectedly!");
} finally
{
    Log.CloseAndFlush();
}