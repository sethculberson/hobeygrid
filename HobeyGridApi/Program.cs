using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using HobeyGridApi.Data;
using HobeyGridApi.Models;
using HobeyGridApi.Services;
using HobeyGridApi.Auth;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using HobeyGridApi.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Check for the WEBSITES_PORT environment variable and configure the app to listen on it.
var port = Environment.GetEnvironmentVariable("WEBSITES_PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://+:{port}");
}
else
{
    // Use the default port from appsettings.json or the project if not running on Azure.
    builder.WebHost.UseUrls("http://+:" + (builder.Configuration["ASPNETCORE_URLS"] ?? "8080"));
}

// Define a CORS policy
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("https://hobeygrid.com", "https://www.hobeygrid.com, https://proud-coast-029e37910.2.azurestaticapps.net")
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
});

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // Add converter for DateOnly for correct serialization/deserialization
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

// Configure PostgreSQL with Npgsql and EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        })
);

// Register GridGenerationService for Dependency Injection
builder.Services.AddScoped<GridGenerationService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use the CORS policy - IMPORTANT: This must be placed before UseAuthorization() and UseEndpoints()
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();