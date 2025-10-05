using Microsoft.EntityFrameworkCore;
using HotelApi.Models;
using HotelApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Usar base de datos en memoria para pruebas
builder.Services.AddDbContext<HotelContext>(options =>
    options.UseInMemoryDatabase("HotelDB"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
