using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HotelManagement.Services;
using HotelManagement.Presentacion.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelManagement.Tests.Controller
{
    [TestClass]
    public class DeleteTests
    {
        // Declaramos las variables que usaremos en las pruebas
        private Mock<IClienteService> _mockClienteService;
        private ClienteController _controller;

        // Este método se ejecuta antes de cada prueba para inicializar los objetos
        [TestInitialize]
        public void Setup()
        {
            // Creamos un mock (simulador) del servicio de cliente
            _mockClienteService = new Mock<IClienteService>();
            // Creamos una instancia del controlador, inyectándole el servicio simulado
            _controller = new ClienteController(_mockClienteService.Object);
        }

        #region Pruebas para el método Delete

        [TestMethod]
        [TestCategory("Pruebas de Caja Blanca: Delete")]
        public async Task Delete_ConIdExistente_DebeRetornarNoContent()
        {
            // --- ARRANGE (Organizar) ---
            // Definimos un ID de cliente válido para la prueba
            var clienteIdExistente = "a1b2c3d4";

            // Configuramos el mock para que cuando se llame a DeleteAsync con este ID,
            // devuelva 'true', simulando que el borrado fue exitoso.
            _mockClienteService.Setup(service => service.DeleteAsync(clienteIdExistente))
                               .ReturnsAsync(true);

            // --- ACT (Actuar) ---
            // Ejecutamos el método Delete del controlador
            var resultado = await _controller.Delete(clienteIdExistente);

            // --- ASSERT (Afirmar) ---
            // Verificamos que el resultado sea del tipo 'NoContentResult' (código 204)
            Assert.IsInstanceOfType(resultado, typeof(NoContentResult));
        }

        [TestMethod]
        [TestCategory("Pruebas de Caja Blanca: Delete")]
        public async Task Delete_ConIdInexistente_DebeRetornarNotFound()
        {
            // --- ARRANGE (Organizar) ---
            // Definimos un ID de cliente que no existe
            var clienteIdInexistente = "e5f6g7h8";

            // Configuramos el mock para que cuando se llame a DeleteAsync con este ID,
            // devuelva 'false', simulando que el cliente no se encontró.
            _mockClienteService.Setup(service => service.DeleteAsync(clienteIdInexistente))
                               .ReturnsAsync(false);

            // --- ACT (Actuar) ---
            // Ejecutamos el método Delete del controlador
            var resultado = await _controller.Delete(clienteIdInexistente);

            // --- ASSERT (Afirmar) ---
            // Verificamos que el resultado sea del tipo 'NotFoundResult' (código 404)
            Assert.IsInstanceOfType(resultado, typeof(NotFoundResult));
        }

        #endregion
    }
}