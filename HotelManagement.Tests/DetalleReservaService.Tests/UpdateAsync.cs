using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HotelManagement.Services;
using HotelManagement.Repositories;
using HotelManagement.Aplicacion.Validators;
using HotelManagement.Models;
using HotelManagement.DTOs;
using HotelManagement.Aplicacion.Exceptions;
using System;
using System.Threading.Tasks;

namespace HotelManagement.Tests.DetallerReserva.Service
{
    [TestClass]
    public class UpdateAsyncTests
    {
        private Mock<IDetalleReservaRepository> _mockRepository;
        private Mock<IDetalleReservaValidator> _mockValidator;
        private DetalleReservaService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IDetalleReservaRepository>();
            _mockValidator = new Mock<IDetalleReservaValidator>();
            _service = new DetalleReservaService(_mockRepository.Object, _mockValidator.Object);
        }

        // Test 1: El detalle de reserva no se encuentra y lanza NotFoundException.
        [TestMethod]
        public async Task UpdateAsync_DetalleNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var dto = new DetalleReservaUpdateDTO();
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<byte[]>())).ReturnsAsync((DetalleReserva)null);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => _service.UpdateAsync(id, dto));
        }

        // Test 2: Actualización exitosa con todos los campos proporcionados.
        [TestMethod]
        public async Task UpdateAsync_AllFieldsProvided_UpdatesSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var detalleExistente = new DetalleReserva
            {
                ID = id.ToByteArray(),
                Habitacion_ID = Guid.NewGuid().ToByteArray(),
                Huesped_ID = Guid.NewGuid().ToByteArray(),
                Precio_Total = 150,
                Cantidad_Huespedes = 1
            };

            var dto = new DetalleReservaUpdateDTO
            {
                Habitacion_ID = Guid.NewGuid().ToString(),
                Huesped_ID = Guid.NewGuid().ToString(),
                Precio_Total = 200,
                Cantidad_Huespedes = 2
            };

            _mockRepository.Setup(r => r.GetByIdAsync(id.ToByteArray()))
                .ReturnsAsync(detalleExistente);

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<DetalleReserva>()))
                .ReturnsAsync(detalleExistente);

            // Act
            var result = await _service.UpdateAsync(id.ToString(), dto);

            // Assert
            Assert.IsNotNull(result);
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<DetalleReserva>(d =>
                new Guid(d.Habitacion_ID).ToString() == dto.Habitacion_ID &&
                new Guid(d.Huesped_ID).ToString() == dto.Huesped_ID &&
                d.Precio_Total == dto.Precio_Total &&
                d.Cantidad_Huespedes == dto.Cantidad_Huespedes
            )), Times.Once);
        }

        // Test 3: Actualización exitosa solo con Habitacion_ID.
        [TestMethod]
        public async Task UpdateAsync_OnlyHabitacionIdProvided_UpdatesSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var detalleExistente = new DetalleReserva
            {
                ID = id.ToByteArray(),
                Habitacion_ID = Guid.NewGuid().ToByteArray(),
                Huesped_ID = Guid.NewGuid().ToByteArray(),
                Precio_Total = 100,
                Cantidad_Huespedes = 2
            };
            var dto = new DetalleReservaUpdateDTO { Habitacion_ID = Guid.NewGuid().ToString() };

            _mockRepository.Setup(repo => repo.GetByIdAsync(id.ToByteArray())).ReturnsAsync(detalleExistente);
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<DetalleReserva>())).ReturnsAsync(detalleExistente);

            // Act
            await _service.UpdateAsync(id.ToString(), dto);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateAsync(It.Is<DetalleReserva>(d =>
                new Guid(d.Habitacion_ID).ToString() == dto.Habitacion_ID
            )), Times.Once);
        }

        // Test 4: Actualización exitosa solo con Huesped_ID.
        [TestMethod]
        public async Task UpdateAsync_OnlyHuespedIdProvided_UpdatesSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var detalleExistente = new DetalleReserva
            {
                ID = id.ToByteArray(),
                Habitacion_ID = Guid.NewGuid().ToByteArray(),
                Huesped_ID = Guid.NewGuid().ToByteArray(),
                Precio_Total = 100,
                Cantidad_Huespedes = 2
            };
            var dto = new DetalleReservaUpdateDTO { Huesped_ID = Guid.NewGuid().ToString() };

            _mockRepository.Setup(repo => repo.GetByIdAsync(id.ToByteArray())).ReturnsAsync(detalleExistente);
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<DetalleReserva>())).ReturnsAsync(detalleExistente);

            // Act
            await _service.UpdateAsync(id.ToString(), dto);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateAsync(It.Is<DetalleReserva>(d =>
                new Guid(d.Huesped_ID).ToString() == dto.Huesped_ID
            )), Times.Once);
        }

        // Test 5: Actualización exitosa solo con Precio_Total.
        [TestMethod]
        public async Task UpdateAsync_OnlyPrecioTotalProvided_UpdatesSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var detalleExistente = new DetalleReserva
            {
                ID = id.ToByteArray(),
                Habitacion_ID = Guid.NewGuid().ToByteArray(),
                Huesped_ID = Guid.NewGuid().ToByteArray(),
                Precio_Total = 100,
                Cantidad_Huespedes = 2
            };
            var dto = new DetalleReservaUpdateDTO { Precio_Total = 300 };

            _mockRepository.Setup(repo => repo.GetByIdAsync(id.ToByteArray())).ReturnsAsync(detalleExistente);
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<DetalleReserva>())).ReturnsAsync(detalleExistente);

            // Act
            await _service.UpdateAsync(id.ToString(), dto);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateAsync(It.Is<DetalleReserva>(d =>
                d.Precio_Total == dto.Precio_Total
            )), Times.Once);
        }

        // Test 6: Actualización exitosa solo con Cantidad_Huespedes.
        [TestMethod]
        public async Task UpdateAsync_OnlyCantidadHuespedesProvided_UpdatesSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var detalleExistente = new DetalleReserva
            {
                ID = id.ToByteArray(),
                Habitacion_ID = Guid.NewGuid().ToByteArray(),
                Huesped_ID = Guid.NewGuid().ToByteArray(),
                Precio_Total = 100,
                Cantidad_Huespedes = 2
            };
            var dto = new DetalleReservaUpdateDTO { Cantidad_Huespedes = 3 };

            _mockRepository.Setup(repo => repo.GetByIdAsync(id.ToByteArray())).ReturnsAsync(detalleExistente);
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<DetalleReserva>())).ReturnsAsync(detalleExistente);

            // Act
            await _service.UpdateAsync(id.ToString(), dto);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateAsync(It.Is<DetalleReserva>(d =>
                d.Cantidad_Huespedes == dto.Cantidad_Huespedes
            )), Times.Once);
        }
    }
}