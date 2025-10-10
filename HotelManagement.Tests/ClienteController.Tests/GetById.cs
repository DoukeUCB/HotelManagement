using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HotelManagement.Services;
using HotelManagement.Presentacion.Controllers;
using HotelManagement.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net;

namespace HotelManagement.Tests.Controller
{
    [TestClass]
    public class GetByIdTests
    {
        private Mock<IClienteService> _mockClienteService;
        private ClienteController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Arrange (común para todas las pruebas)
            _mockClienteService = new Mock<IClienteService>();
            _controller = new ClienteController(_mockClienteService.Object);
        }

        [TestMethod]
        public async Task GetById_ConIdExistente_DebeRetornarOkConCliente()
        {
            // Arrange
            var clienteId = "1";
            var clienteDto = new ClienteDTO { ID = clienteId, Razon_Social = "Juan Perez", Email = "juan@example.com" };
            _mockClienteService.Setup(service => service.GetByIdAsync(clienteId))
                               .ReturnsAsync(clienteDto);

            // Act
            var actionResult = await _controller.GetById(clienteId);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            var okResult = actionResult as OkObjectResult;
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);

            var clienteRetornado = okResult.Value as ClienteDTO;
            Assert.IsNotNull(clienteRetornado);
            Assert.AreEqual(clienteId, clienteRetornado.ID);
            Assert.AreEqual("Juan Perez", clienteRetornado.Razon_Social);
        }

        [TestMethod]
        public async Task GetById_ConIdInexistente_DebeRetornarOkConNull()
        {
            // Arrange
            var clienteId = "99";
            // Configuramos el mock para que devuelva null, simulando que el cliente no se encontró.
            _mockClienteService.Setup(service => service.GetByIdAsync(clienteId))
                               .ReturnsAsync((ClienteDTO)null);

            // Act
            var actionResult = await _controller.GetById(clienteId);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            var okResult = actionResult as OkObjectResult;
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
            // El comportamiento actual es devolver OK con un cuerpo nulo.
            // Una implementación alternativa podría devolver NotFoundResult.
            Assert.IsNull(okResult.Value);
        }
    }
}