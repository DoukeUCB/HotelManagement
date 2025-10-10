using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using HotelManagement.Controllers;
using HotelManagement.DTOs;
using HotelManagement.Services;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace HotelManagement.Tests.Controller.DetalleReserva
{
    [TestClass]
    public class UpdaControllerTests
    {
        private Mock<IDetalleReservaService> _mockService;
        private Mock<ILogger<DetalleReservaController>> _mockLogger;
        private DetalleReservaController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Inicializaci�n com�n para todas las pruebas
            _mockService = new Mock<IDetalleReservaService>();
            _mockLogger = new Mock<ILogger<DetalleReservaController>>();
            _controller = new DetalleReservaController(_mockService.Object, _mockLogger.Object);
        }

        // --- Pruebas para el m�todo Update ---

        [TestMethod]
        public async Task Update_ConIdYDtoValidos_DebeRetornarOkConDetalleActualizado()
        {
            // Arrange (Preparar)
            var testId = "a1b2c3d4";
            var updateDto = new DetalleReservaUpdateDTO { /* ... inicializa sus propiedades ... */ };
            var expectedDto = new DetalleReservaDTO { ID = testId, /* ... resto de propiedades ... */ };

            _mockService.Setup(s => s.UpdateAsync(testId, updateDto))
                        .ReturnsAsync(expectedDto);

            // Act (Actuar)
            var result = await _controller.Update(testId, updateDto);

            // Assert (Afirmar)
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedDto = okResult.Value as DetalleReservaDTO;
            Assert.IsNotNull(returnedDto);
            Assert.AreEqual(expectedDto.ID, returnedDto.ID);
        }

        [TestMethod]
        public async Task Update_CuandoIdNoExiste_DebeLanzarKeyNotFoundException()
        {
            // Arrange (Preparar)
            var testId = "id-no-existente";
            var updateDto = new DetalleReservaUpdateDTO { /* ... */ };

            _mockService.Setup(s => s.UpdateAsync(testId, updateDto))
                        .ThrowsAsync(new KeyNotFoundException("Detalle de reserva no encontrado"));

            // Act & Assert (Actuar y Afirmar)
            // Verificamos que se lance la excepci�n esperada.
            // En un escenario real, un middleware de manejo de errores convertir�a esto en un 404.
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _controller.Update(testId, updateDto));
        }

        [TestMethod]
        public async Task Update_ConDatosInvalidos_DebeLanzarArgumentException()
        {
            // Arrange (Preparar)
            var testId = "a1b2c3d4";
            var updateDto = new DetalleReservaUpdateDTO { /* ... datos inv�lidos ... */ };

            _mockService.Setup(s => s.UpdateAsync(testId, updateDto))
                        .ThrowsAsync(new ArgumentException("Los datos proporcionados son inv�lidos."));

            // Act & Assert (Actuar y Afirmar)
            // Verificamos que el servicio lance la excepci�n correcta.
            // El middleware lo convertir�a en un 400 Bad Request.
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _controller.Update(testId, updateDto));
        }
    }
}