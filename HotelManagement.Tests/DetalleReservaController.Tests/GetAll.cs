using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using HotelManagement.Controllers;
using HotelManagement.DTOs;
using HotelManagement.Services;

namespace HotelManagement.Tests.DetallerReserva.Controller;

[TestClass]
public class DetalleReservaController_GetAllAsync_Tests
{
    // Mocks para las dependencias del controlador
    private readonly Mock<IDetalleReservaService> _mockService;
    private readonly Mock<ILogger<DetalleReservaController>> _mockLogger;

    // La instancia del controlador que vamos a probar
    private readonly DetalleReservaController _controller;

    // Constructor: se ejecuta antes de cada prueba para inicializar los objetos
    public DetalleReservaController_GetAllAsync_Tests()
    {
        _mockService = new Mock<IDetalleReservaService>();
        _mockLogger = new Mock<ILogger<DetalleReservaController>>();
        _controller = new DetalleReservaController(_mockService.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task GetAll_DeberiaDevolverOkConListaDeDetalles_CuandoExistenDatos()
    {
        // --- ARRANGE (Organizar) ---

        // 1. Crear una lista de datos de prueba (DTOs) que simulará la respuesta del servicio.
        var detallesDePrueba = new List<DetalleReservaDTO>
        {
            new DetalleReservaDTO { ID = "d-001", Cantidad_Huespedes = 2, Precio_Total = 200 },
            new DetalleReservaDTO { ID = "d-002", Cantidad_Huespedes = 1, Precio_Total = 150 }
        };

        // 2. Configurar el mock del servicio. Cuando se llame al método GetAllAsync(),
        //    devolverá la lista de prueba que acabamos de crear.
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(detallesDePrueba);

        // --- ACT (Actuar) ---

        // 3. Ejecutar el método del controlador que estamos probando.
        var resultado = await _controller.GetAll();

        // --- ASSERT (Afirmar) ---

        // 4. Verificar el tipo de resultado. Esperamos un OkObjectResult, que corresponde a un código HTTP 200 OK.
        //    Usamos 'Should()' de FluentAssertions para una sintaxis más legible.
        resultado.Result.Should().BeOfType<OkObjectResult>();

        // 5. Extraer el valor contenido en el OkObjectResult.
        var okResult = resultado.Result as OkObjectResult;

        // 6. Verificar que el valor no es nulo y que es del tipo esperado (una lista de DTOs).
        okResult.Value.Should().NotBeNull();
        okResult.Value.Should().BeAssignableTo<List<DetalleReservaDTO>>();

        // 7. Verificar que la lista devuelta es idéntica a nuestra lista de prueba.
        var listaDevuelta = okResult.Value as List<DetalleReservaDTO>;
        listaDevuelta.Should().BeEquivalentTo(detallesDePrueba);

        // 8. (Opcional pero recomendado) Verificar que el método del servicio fue llamado exactamente una vez.
        _mockService.Verify(s => s.GetAllAsync(), Times.Once);
    }
}