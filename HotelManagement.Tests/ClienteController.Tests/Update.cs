using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HotelManagement.Services;
using HotelManagement.DTOs;
using HotelManagement.Presentacion.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelManagement.Tests.Controller
{
    [TestClass]
    public class UpdateTests
    {
        private Mock<IClienteService> _mockClienteService;
        private ClienteController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Arrange: Inicializa el mock y el controlador antes de cada prueba
            _mockClienteService = new Mock<IClienteService>();
            _controller = new ClienteController(_mockClienteService.Object);
        }

        [TestMethod]
        public async Task Update_ClienteExistente_DebeRetornarOkConCliente()
        {
            // Arrange
            var clienteId = "1";
            var updateDto = new ClienteUpdateDTO { /* ... inicializar con datos de prueba ... */ };
            var clienteDto = new ClienteDTO { ID = clienteId, /* ... otros datos ... */ };

            _mockClienteService.Setup(s => s.UpdateAsync(clienteId, updateDto))
                .ReturnsAsync(clienteDto);

            // Act
            var result = await _controller.Update(clienteId, updateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(clienteDto, okResult.Value);
        }

        [TestMethod]
        public async Task Update_ClienteNoExistente_DebeLanzarExcepcion()
        {
            // Arrange
            var clienteId = "99"; // Un ID que no existe
            var updateDto = new ClienteUpdateDTO { /* ... datos ... */ };

            _mockClienteService.Setup(s => s.UpdateAsync(clienteId, updateDto))
                .ThrowsAsync(new KeyNotFoundException("Cliente no encontrado"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _controller.Update(clienteId, updateDto)
            );
        }

        [TestMethod]
        public async Task Update_ConDatosInvalidos_DebeLanzarExcepcion()
        {
            // Arrange
            var clienteId = "1";
            var updateDto = new ClienteUpdateDTO { /* ... datos inválidos ... */ };

            _mockClienteService.Setup(s => s.UpdateAsync(clienteId, updateDto))
                .ThrowsAsync(new ArgumentException("Datos inválidos"));

            _controller.ModelState.AddModelError("Error", "Error de modelo de prueba");

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                _controller.Update(clienteId, updateDto)
            );
        }
    }
}