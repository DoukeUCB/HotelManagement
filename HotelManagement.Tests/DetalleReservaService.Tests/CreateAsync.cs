// Aseg�rate de tener las referencias a los proyectos y paquetes necesarios (MSTest, Moq)
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HotelManagement.Services;
using HotelManagement.Repositories;
using HotelManagement.Aplicacion.Validators;
using HotelManagement.DTOs;
using HotelManagement.Models;
using HotelManagement.Aplicacion.Exceptions; // Aseg�rate de que esta referencia sea correcta
using System;
using System.Threading.Tasks;

namespace HotelManagement.Tests.DetallerReserva.Service
{
    [TestClass]
    public class DetalleReservaServiceCreateAsyncTests
    {
        private Mock<IDetalleReservaRepository> _mockRepository;
        private Mock<IDetalleReservaValidator> _mockValidator;
        private DetalleReservaService _service;

        // El m�todo [TestInitialize] se ejecuta antes de cada prueba.
        // Es ideal para configurar el estado inicial que cada prueba necesita.
        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IDetalleReservaRepository>();
            _mockValidator = new Mock<IDetalleReservaValidator>();
            _service = new DetalleReservaService(_mockRepository.Object, _mockValidator.Object);
        }

        // PRUEBA 1: Cubre el "camino feliz" o el flujo exitoso.
        // Complejidad Ciclom�tica - Camino 1
        [TestMethod]
        public async Task CreateAsync_ShouldReturnDetalleReservaDTO_WhenValidationSucceeds()
        {
            // Arrange (Preparar)
            // 1. DTO de entrada con datos v�lidos.
            var createDto = new DetalleReservaCreateDTO
            {
                Reserva_ID = Guid.NewGuid().ToString(),
                Habitacion_ID = Guid.NewGuid().ToString(),
                Huesped_ID = Guid.NewGuid().ToString(),
                Precio_Total = 150.50m,
                Cantidad_Huespedes = 2
            };

            // 2. Objeto que simula lo que el repositorio devolver�.
            var detalleReservaCreado = new DetalleReserva
            {
                ID = Guid.NewGuid().ToByteArray(),
                Reserva_ID = Guid.Parse(createDto.Reserva_ID).ToByteArray(),
                Habitacion_ID = Guid.Parse(createDto.Habitacion_ID).ToByteArray(),
                Huesped_ID = Guid.Parse(createDto.Huesped_ID).ToByteArray(),
                Precio_Total = createDto.Precio_Total,
                Cantidad_Huespedes = createDto.Cantidad_Huespedes
            };

            // 3. Configurar los mocks.
            //    - El validador no har� nada (simulando que la validaci�n es exitosa).
            _mockValidator.Setup(v => v.ValidateCreateAsync(createDto)).Returns(Task.CompletedTask);

            //    - El repositorio devolver� el objeto creado cuando se llame a CreateAsync.
            //      It.IsAny<DetalleReserva>() se usa porque el ID se genera dentro del m�todo,
            //      por lo que no podemos pasar el objeto exacto.
            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<DetalleReserva>())).ReturnsAsync(detalleReservaCreado);

            // Act (Actuar)
            var result = await _service.CreateAsync(createDto);

            // Assert (Afirmar)
            // 1. Verificar que el resultado no es nulo.
            Assert.IsNotNull(result);

            // 2. Verificar que el tipo de resultado es el esperado.
            Assert.IsInstanceOfType(result, typeof(DetalleReservaDTO));

            // 3. Verificar que los datos mapeados en el DTO son correctos.
            Assert.AreEqual(createDto.Reserva_ID, result.Reserva_ID);
            Assert.AreEqual(createDto.Precio_Total, result.Precio_Total);

            // 4. Verificar que los m�todos de los mocks fueron llamados como se esperaba.
            //    - Se llam� al validador una vez.
            _mockValidator.Verify(v => v.ValidateCreateAsync(createDto), Times.Once);
            //    - Se llam� al m�todo CreateAsync del repositorio una vez.
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<DetalleReserva>()), Times.Once);
        }

        // PRUEBA 2: Cubre el camino donde la validaci�n falla.
        // Complejidad Ciclom�tica - Camino 2
        [TestMethod]
        public async Task CreateAsync_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange (Preparar)
            // 1. DTO de entrada (puede estar vac�o, ya que la l�gica de validaci�n est� en el mock).
            var createDto = new DetalleReservaCreateDTO();

            // 2. Configurar el mock del validador para que lance una excepci�n.
            var validationException = new ValidationException("Error de validaci�n");
            _mockValidator.Setup(v => v.ValidateCreateAsync(createDto)).ThrowsAsync(validationException);

            // Act & Assert (Actuar y Afirmar)
            // Usamos Assert.ThrowsExceptionAsync para verificar que se lanz� la excepci�n esperada.
            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(async () =>
            {
                await _service.CreateAsync(createDto);
            });

            // Afirmaciones adicionales sobre la excepci�n
            Assert.AreEqual("Error de validaci�n", exception.Message);

            // Verificar que el m�todo del repositorio NUNCA fue llamado, ya que la validaci�n fall� antes.
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<DetalleReserva>()), Times.Never);
        }
    }
}