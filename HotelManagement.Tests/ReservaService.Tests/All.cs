using FluentAssertions;
using HotelManagement.Application.Services;
using HotelManagement.Datos.Config;
using HotelManagement.Datos.Repositories;
using HotelManagement.DTOs;
using HotelManagement.Models;
using HotelManagement.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;


namespace HotelManagement.Tests.Reserva.Service;
[TestClass]
public class ReservaServiceTests
{
    private readonly Mock<IReservaRepository> _mockReservaRepository;
    private readonly Mock<IClienteRepository> _mockClienteRepository;
    private readonly HotelDbContext _context;
    private readonly ReservaService _service;

    public ReservaServiceTests()
    {
        // Configuración de Mocks
        _mockReservaRepository = new Mock<IReservaRepository>();
        _mockClienteRepository = new Mock<IClienteRepository>();

        // Configuración de DbContext en memoria
        var options = new DbContextOptionsBuilder<HotelDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Base de datos única para cada test
            .Options;
        _context = new HotelDbContext(options);

        // Instancia del servicio bajo prueba
        _service = new ReservaService(
            _mockReservaRepository.Object,
            _mockClienteRepository.Object,
            _context);
    }

    // --- Pruebas para GetAllAsync ---

    [TestMethod]
    public async Task GetAllAsync_DeberiaRetornarListaDeReservasDTO()
    {
        // Arrange
        var clienteId = Guid.NewGuid().ToByteArray();
        var reserva1Id = Guid.NewGuid();
        var reserva2Id = Guid.NewGuid();

        var reservas = new List<HotelManagement.Models.Reserva>

        {
            new HotelManagement.Models.Reserva { ID = reserva1Id.ToByteArray(), Cliente_ID = clienteId, Cliente = new Cliente { Razon_Social = "Cliente 1" } },
            new HotelManagement.Models.Reserva { ID = reserva2Id.ToByteArray(), Cliente_ID = clienteId, Cliente = new Cliente { Razon_Social = "Cliente 1" } }
        };

        var detalles = new List<DetalleReserva>
    {
        // ✨ SOLUCIÓN: Agrega los IDs requeridos
        new DetalleReserva
        {
            ID = Guid.NewGuid().ToByteArray(),
            Reserva_ID = reserva1Id.ToByteArray(),
            Habitacion_ID = Guid.NewGuid().ToByteArray(), // <-- Agregado
            Huesped_ID = Guid.NewGuid().ToByteArray(),    // <-- Agregado
            Fecha_Entrada = DateTime.Now,
            Fecha_Salida = DateTime.Now.AddDays(2)
        },
        new DetalleReserva
        {
            ID = Guid.NewGuid().ToByteArray(),
            Reserva_ID = reserva2Id.ToByteArray(),
            Habitacion_ID = Guid.NewGuid().ToByteArray(), // <-- Agregado
            Huesped_ID = Guid.NewGuid().ToByteArray(),    // <-- Agregado
            Fecha_Entrada = DateTime.Now.AddDays(5),
            Fecha_Salida = DateTime.Now.AddDays(7)
        }
    };

        _context.DetalleReservas.AddRange(detalles);
        await _context.SaveChangesAsync();

        _mockReservaRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(reservas);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().ID.Should().Be(reserva1Id.ToString());
        result.First().Fecha_Entrada.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetByIdAsync_DeberiaRetornarReservaDTO_CuandoReservaExiste()
    {
        // Arrange
        var reservaId = Guid.NewGuid();
        var reserva = new HotelManagement.Models.Reserva
        {
            ID = reservaId.ToByteArray(),
            Cliente_ID = Guid.NewGuid().ToByteArray(),
            Cliente = new Cliente { Razon_Social = "Hotel Tech" },
            Monto_Total = 500
        };

        _mockReservaRepository.Setup(r => r.GetByIdAsync(reservaId.ToByteArray())).ReturnsAsync(reserva);

        // Act
        var result = await _service.GetByIdAsync(reservaId);

        // Assert
        result.Should().NotBeNull();
        result.ID.Should().Be(reservaId.ToString());
        result.Cliente_Nombre.Should().Be("Hotel Tech");
    }

    [TestMethod]
    public async Task GetByIdAsync_DeberiaRetornarNull_CuandoReservaNoExiste()
    {
        // Arrange
        var reservaId = Guid.NewGuid();
        _mockReservaRepository.Setup(r => r.GetByIdAsync(It.IsAny<byte[]>())).ReturnsAsync((HotelManagement.Models.Reserva)null);

        // Act
        var result = await _service.GetByIdAsync(reservaId);

        // Assert
        result.Should().BeNull();
    }

    // --- Pruebas para AddAsync ---

    [TestMethod]
    public async Task AddAsync_DeberiaAgregarReserva_CuandoClienteExiste()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var dto = new ReservaCreateDTO
        {
            Cliente_ID = clienteId.ToString(),
            Estado_Reserva = "Confirmada",
            Monto_Total = 1200
        };

        var cliente = new Cliente { ID = clienteId.ToByteArray() };
        _mockClienteRepository.Setup(c => c.GetByIdAsync(clienteId.ToByteArray())).ReturnsAsync(cliente);

        // Act
        await _service.AddAsync(dto);

        // Assert
        _mockReservaRepository.Verify(r => r.AddAsync(It.IsAny<HotelManagement.Models.Reserva>()), Times.Once);
        _mockReservaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    // --- Pruebas para AddAsync ---
    [TestMethod]
    public async Task AddAsync_DeberiaLanzarNotFoundException_CuandoClienteNoExiste()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var dto = new ReservaCreateDTO { Cliente_ID = clienteId.ToString() };

        _mockClienteRepository.Setup(c => c.GetByIdAsync(It.IsAny<byte[]>())).ReturnsAsync((Cliente)null);

        // Act & Assert
    }



    // --- Pruebas para UpdateAsync ---

    [TestMethod]
    public async Task UpdateAsync_DeberiaActualizarReserva_CuandoReservaExiste()
    {
        // Arrange
        var reservaId = Guid.NewGuid();
        var reservaExistente = new HotelManagement.Models.Reserva { ID = reservaId.ToByteArray() };
        var dto = new ReservaUpdateDTO { Estado_Reserva = "Cancelada", Monto_Total = 0 };

        _mockReservaRepository.Setup(r => r.GetByIdAsync(reservaId.ToByteArray())).ReturnsAsync(reservaExistente);

        // Act
        var result = await _service.UpdateAsync(reservaId, dto);

        // Assert
        result.Should().BeTrue();
        _mockReservaRepository.Verify(r => r.UpdateAsync(It.Is<HotelManagement.Models.Reserva>(res => res.Estado_Reserva == "Cancelada")), Times.Once);
        _mockReservaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task UpdateAsync_DeberiaRetornarFalse_CuandoReservaNoExiste()
    {
        // Arrange
        var reservaId = Guid.NewGuid();
        var dto = new ReservaUpdateDTO();
        _mockReservaRepository.Setup(r => r.GetByIdAsync(It.IsAny<byte[]>())).ReturnsAsync((HotelManagement.Models.Reserva)null);

        // Act
        var result = await _service.UpdateAsync(reservaId, dto);

        // Assert
        result.Should().BeFalse();
    }
    [TestMethod]
    [Fact]
    public async Task DeleteAsync_DeberiaEliminarReserva_CuandoReservaExiste()
    {
        // Arrange
        var reservaId = Guid.NewGuid();
        var reserva = new HotelManagement.Models.Reserva { ID = reservaId.ToByteArray() };

        _mockReservaRepository.Setup(r => r.GetByIdAsync(reservaId.ToByteArray())).ReturnsAsync(reserva);

        // Act
        var result = await _service.DeleteAsync(reservaId);

        // Assert
        result.Should().BeTrue();
        _mockReservaRepository.Verify(r => r.DeleteAsync(reservaId.ToByteArray()), Times.Once);
        _mockReservaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task DeleteAsync_DeberiaRetornarFalse_CuandoReservaNoExiste()
    {
        // Arrange
        var reservaId = Guid.NewGuid();
        _mockReservaRepository.Setup(r => r.GetByIdAsync(It.IsAny<byte[]>())).ReturnsAsync((HotelManagement.Models.Reserva)null);

        // Act
        var result = await _service.DeleteAsync(reservaId);

        // Assert
        result.Should().BeFalse();
    }

    // --- Pruebas para DeleteAsync ---

}