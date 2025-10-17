using HotelManagement.Datos.Config;
using HotelManagement.Models;
using HotelManagement.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
namespace HotelManagement.Tests.DetallerReserva.Data;
[TestClass]
public class DetalleReservaRepository_CreateAsync_Tests
{
    private DbContextOptions<HotelDbContext> _options;
    private HotelDbContext _context;
    private DetalleReservaRepository _repository;

    [TestInitialize]
    public void Setup()
    {
        // 1. Configurar una base de datos en memoria única para cada prueba
        _options = new DbContextOptionsBuilder<HotelDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // 2. Crear una nueva instancia del contexto y el repositorio
        _context = new HotelDbContext(_options);
        _repository = new DetalleReservaRepository(_context);
    }

    [TestMethod]
    [TestCategory("Pruebas de Caja Blanca - Repositorio")]
    public async Task CreateAsync_CuandoSePasaUnDetalleReservaValido_DebeGuardarloYRetornarlo()
    {
        // Arrange: Preparar el escenario de la prueba
        var nuevoDetalleReserva = new DetalleReserva
        {
            ID = Guid.NewGuid().ToByteArray(),
            Reserva_ID = Guid.NewGuid().ToByteArray(),
            Habitacion_ID = Guid.NewGuid().ToByteArray(),
            Huesped_ID = Guid.NewGuid().ToByteArray()
        };

        // Act: Ejecutar el método que se está probando
        var resultado = await _repository.CreateAsync(nuevoDetalleReserva);

        // Assert: Verificar que el resultado es el esperado
        // 1. Verificar que el método retornó el objeto que se le pasó
        Assert.IsNotNull(resultado);
        Assert.AreEqual(nuevoDetalleReserva.ID, resultado.ID, "El ID del detalle de reserva retornado no es el esperado.");

        // 2. Verificar que el objeto fue realmente guardado en la base de datos (en memoria)
        var detalleGuardado = await _context.DetalleReservas.FindAsync(nuevoDetalleReserva.ID);
        Assert.IsNotNull(detalleGuardado, "El detalle de la reserva no se encontró en la base de datos después de crearlo.");
        Assert.AreEqual(nuevoDetalleReserva.ID, detalleGuardado.ID, "El ID del detalle guardado en la BD no coincide.");
    }
}


