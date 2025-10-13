using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HotelManagement.Services;
using HotelManagement.Repositories;
using HotelManagement.Aplicacion.Validators;
using HotelManagement.DTOs;
using HotelManagement.Models;
using HotelManagement.Aplicacion.Exceptions;
using System;
using System.Threading.Tasks;

namespace HotelManagement.Tests.Services
{
    [TestClass]
    public sealed class GetByAsyncTests
    {
        private Mock<IClienteRepository> _mockRepository;
        private Mock<IClienteValidator> _mockValidator;
        private ClienteService _service;

        // El método [TestInitialize] se ejecuta antes de cada prueba.
        // Es ideal para configurar el estado inicial que cada prueba necesita.
        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IClienteRepository>();
            _mockValidator = new Mock<IClienteValidator>(); // Aunque no se usa en GetByIdAsync, es bueno tenerlo para otras pruebas.
            _service = new ClienteService(_mockRepository.Object, _mockValidator.Object);
        }

        #region Pruebas para GetByIdAsync

        // Prueba para el Camino 1: ID inválido
        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task GetByIdAsync_ConIdInvalido_DebeLanzarBadRequestException()
        {
            // Arrange
            var idInvalido = "esto-no-es-un-guid";

            // Act
            await _service.GetByIdAsync(idInvalido);

            // Assert
            // El assert es manejado por el atributo [ExpectedException]
        }

        // Prueba para el Camino 2: Cliente no encontrado
        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public async Task GetByIdAsync_CuandoClienteNoExiste_DebeLanzarNotFoundException()
        {
            // Arrange
            var idValido = Guid.NewGuid().ToString();
            var guidBytes = Guid.Parse(idValido).ToByteArray();

            // Configuramos el mock para que devuelva null cuando se llame a GetByIdAsync
            _mockRepository.Setup(repo => repo.GetByIdAsync(guidBytes))
                           .ReturnsAsync((Cliente)null);

            // Act
            await _service.GetByIdAsync(idValido);

            // Assert
            // El assert es manejado por el atributo [ExpectedException]
        }

        // Prueba para el Camino 3: Camino feliz
        [TestMethod]
        public async Task GetByIdAsync_CuandoClienteExiste_DebeRetornarClienteDTO()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var clienteIdString = clienteId.ToString();
            var clienteIdBytes = clienteId.ToByteArray();

            var clienteEncontrado = new Cliente
            {
                ID = clienteIdBytes,
                Razon_Social = "Cliente de Prueba",
                NIT = "123456789",
                Email = "test@cliente.com"
            };

            // Configuramos el mock para que devuelva el cliente de prueba
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<byte[]>()))
                           .ReturnsAsync(clienteEncontrado);

            // Act
            var resultado = await _service.GetByIdAsync(clienteIdString);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.IsInstanceOfType(resultado, typeof(ClienteDTO));
            Assert.AreEqual(clienteIdString, resultado.ID);
            Assert.AreEqual("Cliente de Prueba", resultado.Razon_Social);
        }

        #endregion
    }
}