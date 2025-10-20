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
    public class DeleteAsyncTests
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

        #region Pruebas para DeleteAsync

        // Test 1: Cubre el único camino posible
        [TestMethod]
        public async Task DeleteAsync_LlamaValidadorYRepositorio_DevuelveResultado()
        {
            // Arrange
            var clienteId = Guid.NewGuid().ToString();
            var idBytes = Guid.Parse(clienteId).ToByteArray();

            // Configurar el validador para que se complete sin errores
            _mockValidator.Setup(v => v.ValidateDeleteAsync(clienteId)).Returns(Task.CompletedTask);
            // Configurar el repositorio para que devuelva 'true'
            _mockRepository.Setup(r => r.DeleteAsync(idBytes)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(clienteId);

            // Assert
            // Verificar que el validador fue llamado una vez con el ID correcto
            _mockValidator.Verify(v => v.ValidateDeleteAsync(clienteId), Times.Once);
            // Verificar que el repositorio fue llamado una vez con los bytes del ID correcto
            _mockRepository.Verify(r => r.DeleteAsync(idBytes), Times.Once);
            // Verificar que el resultado es el esperado
            Assert.IsTrue(result);
        }

        #endregion
    }
}