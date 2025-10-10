// Aseg�rate de tener los usings necesarios
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using HotelManagement.Controllers;
using HotelManagement.DTOs;
using HotelManagement.Services;
using System.Threading.Tasks;

namespace HotelManagement.Tests.DetallerReserva.Controller;
[TestClass]
public class Create_Tests
{
    // Mocks para las dependencias del controlador
    private Mock<IDetalleReservaService> _mockService;
    private Mock<ILogger<DetalleReservaController>> _mockLogger;
    private DetalleReservaController _controller;

    // El m�todo [TestInitialize] se ejecuta antes de cada prueba.
    // Es ideal para inicializar los objetos que se usar�n en todas las pruebas.
    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<IDetalleReservaService>();
        _mockLogger = new Mock<ILogger<DetalleReservaController>>();
        _controller = new DetalleReservaController(_mockService.Object, _mockLogger.Object);
    }

    #region Pruebas para el m�todo Create

    [TestMethod]
    public async Task Create_CuandoElDTOEsValido_DebeRetornar201Created()
    {
        // --- ARRANGE (Organizar) ---

        // 1. Creamos el objeto de entrada para el m�todo.
        var createDto = new DetalleReservaCreateDTO
        {
            // Asigna aqu� las propiedades necesarias para la creaci�n
            // Ejemplo:
            // ReservaID = "reserva-123",
            // HabitacionID = "hab-101",
            // Precio = 150.00m
        };

        // 2. Creamos el objeto que esperamos que el servicio devuelva.
        var expectedDto = new DetalleReservaDTO
        {
            ID = "nuevo-id-generado",
            // Asigna aqu� las dem�s propiedades que coincidan con createDto
        };

        // 3. Configuramos el mock del servicio:
        //    Cuando se llame a CreateAsync con cualquier DetalleReservaCreateDTO,
        //    devolver� el objeto 'expectedDto' que creamos.
        _mockService.Setup(s => s.CreateAsync(It.IsAny<DetalleReservaCreateDTO>()))
                    .ReturnsAsync(expectedDto);

        // --- ACT (Actuar) ---

        // Ejecutamos el m�todo del controlador que queremos probar.
        var result = await _controller.Create(createDto);

        // --- ASSERT (Afirmar) ---

        // 1. Verificamos que el resultado no sea nulo.
        Assert.IsNotNull(result);

        // 2. Verificamos que el resultado sea del tipo esperado (CreatedAtActionResult).
        //    Este tipo de resultado corresponde a un c�digo de estado HTTP 201.
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);

        // 3. Verificamos que el c�digo de estado sea 201 Created.
        Assert.AreEqual(201, createdResult.StatusCode);

        // 4. Verificamos que el valor devuelto en el cuerpo de la respuesta sea el objeto que esperamos.
        var returnedDto = createdResult.Value as DetalleReservaDTO;
        Assert.IsNotNull(returnedDto);
        Assert.AreEqual(expectedDto.ID, returnedDto.ID);
    }

    [TestMethod]
    public async Task Create_CuandoElServicioLanzaExcepcion_DebePropagarLaExcepcion()
    {
        // --- ARRANGE (Organizar) ---

        // 1. Creamos un DTO de entrada.
        var createDto = new DetalleReservaCreateDTO();

        // 2. Creamos una excepci�n de prueba.
        var exception = new System.InvalidOperationException("Error de prueba en la base de datos");

        // 3. Configuramos el mock del servicio para que lance una excepci�n
        //    cuando se llame a CreateAsync.
        _mockService.Setup(s => s.CreateAsync(It.IsAny<DetalleReservaCreateDTO>()))
                    .ThrowsAsync(exception);

        // --- ACT & ASSERT (Actuar y Afirmar) ---

        // Usamos Assert.ThrowsExceptionAsync para verificar que el m�todo del controlador
        // efectivamente lanza (o propaga) la excepci�n que configuramos en el mock.
        // Esto es importante para asegurar que el middleware de manejo de errores global
        // pueda capturar la excepci�n y devolver una respuesta de error adecuada (ej. 400 o 500).
        var thrownException = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
            () => _controller.Create(createDto)
        );

        // Opcional: Verificar que la excepci�n propagada es la misma que la original.
        Assert.AreEqual(exception.Message, thrownException.Message);
    }

    #endregion
}