using HotelManagement.Services;
using HotelManagement.DTOs;
using HotelManagement.Models;
// Asegúrate de tener la referencia al proyecto principal para acceder a las interfaces
using HotelManagement.Repositories;
using HotelManagement.Aplicacion.Validators;
using HotelManagement.Aplicacion.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace HotelManagement.Tests.Services
{
    [TestClass]
    public class UpdateAsyncTests
    {
        // Declaramos los mocks a nivel de clase para reutilizarlos en los tests
        private Mock<IClienteRepository> _mockRepository;
        private Mock<IClienteValidator> _mockValidator;
        private ClienteService _service;

        // El método [TestInitialize] se ejecuta antes de cada prueba.
        // Es perfecto para instanciar nuestros mocks y el servicio.
        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IClienteRepository>();
            _mockValidator = new Mock<IClienteValidator>();
            _service = new ClienteService(_mockRepository.Object, _mockValidator.Object);
        }

        #region Pruebas para UpdateAsync

        // Test 1: Cubre el camino donde el cliente no se encuentra después de la validación (aunque improbable)
        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public async Task UpdateAsync_CuandoClienteNoExiste_LanzaNotFoundException()
        {
            // Arrange
            var clienteId = Guid.NewGuid().ToString();
            var updateDto = new ClienteUpdateDTO();
            var idBytes = Guid.Parse(clienteId).ToByteArray();

            _mockValidator.Setup(v => v.ValidateUpdateAsync(clienteId, updateDto)).Returns(Task.CompletedTask);
            // Configuramos el repositorio para que devuelva null
            _mockRepository.Setup(r => r.GetByIdAsync(idBytes)).ReturnsAsync((Cliente)null);

            // Act
            await _service.UpdateAsync(clienteId, updateDto);

            // Assert
            // La aserción es la anotación [ExpectedException]
        }

        // Test 2: Cubre el camino donde el DTO no tiene datos para actualizar
        [TestMethod]
        public async Task UpdateAsync_ConDtoVacio_LlamaUpdateConClienteOriginal()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var idString = clienteId.ToString();
            var idBytes = clienteId.ToByteArray();

            var clienteOriginal = new Cliente { ID = idBytes, Razon_Social = "Original", NIT = "123", Email = "a@a.com" };
            var updateDto = new ClienteUpdateDTO { Razon_Social = "", NIT = null, Email = "" }; // DTO vacío

            _mockValidator.Setup(v => v.ValidateUpdateAsync(idString, updateDto)).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.GetByIdAsync(idBytes)).ReturnsAsync(clienteOriginal);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Cliente>())).ReturnsAsync(clienteOriginal);

            // Act
            var result = await _service.UpdateAsync(idString, updateDto);

            // Assert
            // Verificamos que el método UpdateAsync fue llamado con el objeto sin modificar.
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<Cliente>(c =>
                c.Razon_Social == "Original" &&
                c.NIT == "123" &&
                c.Email == "a@a.com"
            )), Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(idString, result.ID);
        }

        // Test 3, 4 y 5: Cubren los caminos donde se actualiza cada campo individualmente
        [TestMethod]
        [DataRow("Nueva Razón Social", null, null, "Nueva Razón Social", "NIT Original", "Email Original")] // Caso 3: Solo Razón Social
        [DataRow(null, "Nuevo NIT", null, "Razón Original", "Nuevo NIT", "Email Original")]                  // Caso 4: Solo NIT
        [DataRow(null, null, "nuevo@email.com", "Razón Original", "NIT Original", "nuevo@email.com")]        // Caso 5: Solo Email
        public async Task UpdateAsync_ActualizaCamposIndividualmente_Correctamente(
            string razonSocial, string nit, string email,
            string expectedRazon, string expectedNit, string expectedEmail)
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var idString = clienteId.ToString();
            var idBytes = clienteId.ToByteArray();

            var clienteOriginal = new Cliente { ID = idBytes, Razon_Social = "Razón Original", NIT = "NIT Original", Email = "Email Original" };
            var updateDto = new ClienteUpdateDTO { Razon_Social = razonSocial, NIT = nit, Email = email };

            _mockValidator.Setup(v => v.ValidateUpdateAsync(idString, updateDto)).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.GetByIdAsync(idBytes)).ReturnsAsync(clienteOriginal);

            // Capturamos el cliente que se pasa al método UpdateAsync para verificarlo
            Cliente clienteActualizado = null;
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Cliente>()))
                           .Callback<Cliente>(c => clienteActualizado = c)
                           .ReturnsAsync(() => clienteActualizado); // Devuelve el cliente actualizado

            // Act
            await _service.UpdateAsync(idString, updateDto);

            // Assert
            Assert.IsNotNull(clienteActualizado);
            Assert.AreEqual(expectedRazon, clienteActualizado.Razon_Social);
            Assert.AreEqual(expectedNit, clienteActualizado.NIT);
            Assert.AreEqual(expectedEmail, clienteActualizado.Email);
        }
        #endregion

    }
}