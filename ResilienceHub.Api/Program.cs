using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using ResilienceHub.Infrastructure.Interfaces;
using ResilienceHub.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
   builder.Services.AddControllers().AddNewtonsoftJson();

// Add services to the container.
builder.Services.AddControllers(options =>
    {
        options.Filters.Add(new ProducesAttribute("application/json"));
        options.Filters.Add(new ConsumesAttribute("application/json"));
    })
    .AddNewtonsoftJson(options => 
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Rate limiting configuration
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

// Database configuration
builder.Services.AddScoped<IDbConnection>(provider =>
{
    var connection = new OracleConnection(builder.Configuration.GetConnectionString("OracleConnection"));
    connection.Open();
    return connection;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ResilienceHub API", Version = "v1" });
    
    // Habilita a documentação XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    
    // Habilita annotations do Swagger
    c.EnableAnnotations();
});

builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("MongoDB");
    return new MongoClient(connectionString);
});

builder.Services.AddScoped<IMongoDatabase>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    return client.GetDatabase("ResilienceHubAnalytics");
});

builder.Services.AddScoped<IAbrigoRepository, AbrigoRepository>();
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRecursoRepository, RecursoRepository>();
builder.Services.AddScoped<IAbrigoUsuarioRepository, AbrigoUsuarioRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIpRateLimiting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();