using Microsoft.VisualStudio.TestTools.UnitTesting;
using HotelManagement.Services;
using Moq;
using HotelManagement.Repositories;
using HotelManagement.Models;
using HotelManagement.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace HotelManagement.Tests.Services
{
    [TestClass]
    public class GetAllAsyncTests
    {
        private Mock<IClienteRepository> _mockRepository;
        private ClienteService _service;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new Mock<IClienteRepository>();
            // No se necesita el validador para GetAllAsync, por lo que se puede pasar como null
            _service = new ClienteService(_mockRepository.Object, null);
        }

        [TestMethod]
        public async Task GetAllAsync_CuandoExistenClientes_DebeRetornarListaDeClientesDTO()
        {
            // Arrange: Configuración de la prueba
            var clientes = new List<Cliente>
            {
                new Cliente { ID = Guid.NewGuid().ToByteArray(), Razon_Social = "Cliente 1", NIT = "123", Email = "c1@test.com" },
                new Cliente { ID = Guid.NewGuid().ToByteArray(), Razon_Social = "Cliente 2", NIT = "456", Email = "c2@test.com" }
            };
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(clientes);

            // Act: Ejecución del método a probar
            var resultado = await _service.GetAllAsync();

            // Assert: Verificación de los resultados
            Assert.IsNotNull(resultado);
            Assert.IsInstanceOfType(resultado, typeof(List<ClienteDTO>));
            Assert.AreEqual(2, resultado.Count);
            Assert.AreEqual("Cliente 1", resultado[0].Razon_Social);
            Assert.AreEqual("Cliente 2", resultado[1].Razon_Social);
        }

        [TestMethod]
        public async Task GetAllAsync_CuandoNoExistenClientes_DebeRetornarListaVacia()
        {
            // Arrange: Configuración de la prueba
            var clientes = new List<Cliente>(); // Lista vacía
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(clientes);

            // Act: Ejecución del método a probar
            var resultado = await _service.GetAllAsync();

            // Assert: Verificación de los resultados
            Assert.IsNotNull(resultado);
            Assert.IsInstanceOfType(resultado, typeof(List<ClienteDTO>));
            Assert.AreEqual(0, resultado.Count);
        }
    }
}