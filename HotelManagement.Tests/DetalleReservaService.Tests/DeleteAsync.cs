// Aseg�rate de tener las referencias a los proyectos y paquetes necesarios (Moq, etc.)
using HotelManagement.Services;
using HotelManagement.Repositories;
using HotelManagement.Aplicacion.Validators;
using HotelManagement.Aplicacion.Exceptions;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace HotelManagement.Tests.DetallerReserva.Service
{
    [TestClass]
    public class DetalleReservaService_DeleteAsync_Tests
    {
        private Mock<IDetalleReservaRepository> _mockRepository;
        private Mock<IDetalleReservaValidator> _mockValidator;
        private DetalleReservaService _service;

        [TestInitialize]
        public void Setup()
        {
            // Inicializamos los mocks y el servicio antes de cada prueba
            _mockRepository = new Mock<IDetalleReservaRepository>();
            _mockValidator = new Mock<IDetalleReservaValidator>();
            _service = new DetalleReservaService(_mockRepository.Object, _mockValidator.Object);
        }

        /// <summary>
        /// Prueba para el camino exitoso (Complejidad Ciclom�tica: 1/1)
        /// Verifica que el m�todo devuelve true cuando el ID es v�lido y la eliminaci�n en el repositorio es exitosa.
        /// </summary>
        [TestMethod]
        public async Task DeleteAsync_ConIdValido_DebeRetornarTrue()
        {
            // Arrange (Preparar)
            var idValido = Guid.NewGuid().ToString();
            var idBytes = Guid.Parse(idValido).ToByteArray();

            // Configuramos el validador para que no haga nada (pase la validaci�n)
            _mockValidator
                .Setup(v => v.ValidateDeleteAsync(idValido))
                .Returns(Task.CompletedTask);

            // Configuramos el repositorio para que simule una eliminaci�n exitosa
            _mockRepository
                .Setup(r => r.DeleteAsync(It.Is<byte[]>(b => b.SequenceEqual(idBytes))))
                .ReturnsAsync(true);

            // Act (Actuar)
            var resultado = await _service.DeleteAsync(idValido);

            // Assert (Afirmar)
            Assert.IsTrue(resultado); // Verificamos que el resultado sea el esperado
            _mockValidator.Verify(v => v.ValidateDeleteAsync(idValido), Times.Once); // Verificamos que el validador fue llamado una vez
            _mockRepository.Verify(r => r.DeleteAsync(idBytes), Times.Once); // Verificamos que el repositorio fue llamado una vez
        }

        /// <summary>
        /// Prueba para el camino de fallo cuando el validador lanza una excepci�n.
        /// Verifica que la excepci�n del validador se propaga correctamente.
        /// </summary>
        [TestMethod]
        public async Task DeleteAsync_CuandoValidadorLanzaExcepcion_DebePropagarLaExcepcion()
        {
            // Arrange
            var idInvalido = "id-no-valido";
            var mensajeExcepcion = $"No se encontr� el detalle de reserva con ID: {idInvalido}";

            // Configuramos el validador para que lance una excepci�n
            _mockValidator
                .Setup(v => v.ValidateDeleteAsync(idInvalido))
                .ThrowsAsync(new NotFoundException(mensajeExcepcion));

            // Act & Assert
            var excepcion = await Assert.ThrowsExceptionAsync<NotFoundException>(
                () => _service.DeleteAsync(idInvalido)
            );

            Assert.AreEqual(mensajeExcepcion, excepcion.Message); // Verificamos que el mensaje de la excepci�n sea el correcto
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<byte[]>()), Times.Never); // Verificamos que el repositorio NUNCA fue llamado
        }

        /// <summary>
        /// Prueba para el camino de fallo cuando el repositorio no logra eliminar el registro.
        /// </summary>
        [TestMethod]
        public async Task DeleteAsync_CuandoRepositorioFalla_DebeRetornarFalse()
        {
            // Arrange
            var idValido = Guid.NewGuid().ToString();
            var idBytes = Guid.Parse(idValido).ToByteArray();

            _mockValidator
                .Setup(v => v.ValidateDeleteAsync(idValido))
                .Returns(Task.CompletedTask);

            // Configuramos el repositorio para que simule una eliminaci�n fallida
            _mockRepository
                .Setup(r => r.DeleteAsync(It.Is<byte[]>(b => b.SequenceEqual(idBytes))))
                .ReturnsAsync(false);

            // Act
            var resultado = await _service.DeleteAsync(idValido);

            // Assert
            Assert.IsFalse(resultado); // Verificamos que el resultado sea false
            _mockValidator.Verify(v => v.ValidateDeleteAsync(idValido), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(idBytes), Times.Once);
        }
    }
}