using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HotelManagement.Services;
using HotelManagement.Presentacion.Controllers;
using HotelManagement.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net;
using System; // Necesario para Exception

namespace HotelManagement.Tests.Controller
{
    [TestClass]
    public class CreateTests
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
        public async Task Create_ConDatosValidos_DebeRetornarCreatedConCliente()
        {
            // Arrange
            var clienteCreateDto = new ClienteCreateDTO { Razon_Social = "Ana Lopez", Email = "ana@example.com", NIT = "12345678" };
            var clienteCreadoDto = new ClienteDTO { ID = "2", Razon_Social = "Ana Lopez", Email = "ana@example.com" };

            _mockClienteService.Setup(service => service.CreateAsync(clienteCreateDto))
                               .ReturnsAsync(clienteCreadoDto);

            // Act
            var actionResult = await _controller.Create(clienteCreateDto);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(CreatedAtActionResult));
            var createdResult = actionResult as CreatedAtActionResult;
            Assert.AreEqual((int)HttpStatusCode.Created, createdResult.StatusCode);

            var clienteRetornado = createdResult.Value as ClienteDTO;
            Assert.IsNotNull(clienteRetornado);
            Assert.AreEqual("2", clienteRetornado.ID);
            Assert.AreEqual("Ana Lopez", clienteRetornado.Razon_Social);
            Assert.AreEqual(nameof(ClienteController.GetById), createdResult.ActionName);
        }

        [TestMethod]
        public async Task Create_ConEmailDuplicado_DebeLanzarExcepcion()
        {
            // Arrange
            var clienteCreateDto = new ClienteCreateDTO { Razon_Social = "Pedro Gomez", Email = "pedro@example.com", NIT = "87654321" };

            // Simulamos que el servicio lanza una excepción por conflicto
            // NOTA: El manejo real (convertir la excepción a un 409 Conflict)
            // usualmente lo hace un Middleware de manejo de errores, no el controlador.
            // Esta prueba verifica que la excepción se propaga desde el controlador.
            _mockClienteService.Setup(service => service.CreateAsync(clienteCreateDto))
                               .ThrowsAsync(new Exception("Simulación de conflicto: el email ya existe."));

            // Act & Assert
            // Verificamos que se lanza una excepción cuando llamamos al método del controlador.
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _controller.Create(clienteCreateDto);
            });
        }
    }
}