using Microsoft.EntityFrameworkCore;
using HotelManagement.Datos.Config;
using HotelManagement.Repositories;
using HotelManagement.Services;
using HotelManagement.Aplicacion.Validators;
using HotelManagement.Presentacion.Middleware;
using DotNetEnv;

// Cargar variables de entorno desde .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configuración de la cadena de conexión
var server = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
var database = Environment.GetEnvironmentVariable("DB_NAME") ?? "HotelDB";
var user = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "1234";

var connectionString = $"Server={server};Port={port};Database={database};User={user};Password={password};";

// Configurar DbContext
builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Registrar servicios
builder.Services.AddScoped<IDetalleReservaRepository, DetalleReservaRepository>();
builder.Services.AddScoped<IDetalleReservaService, DetalleReservaService>();
builder.Services.AddScoped<IDetalleReservaValidator, DetalleReservaValidator>();
//Cliente
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>(); 
builder.Services.AddScoped<IClienteValidator, ClienteValidator>(); 
// Configurar controladores
builder.Services.AddControllers();

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Hotel Management API",
        Version = "v1",
        Description = "API para la gestión de reservas de hotel",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Hotel Management Team"
        }
    });
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware de manejo de errores
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configurar pipeline HTTP
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Management API v1");
    c.RoutePrefix = string.Empty; // http://localhost:5000
});
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
