using HotelManagement.DTOs;
using HotelManagement.Models;
using HotelManagement.Repositories;
using HotelManagement.Services;
using HotelManagement.Aplicacion.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement.Aplicacion.Exceptions;

namespace HotelManagement.Tests.DetallerReserva.Service
{
    [TestClass]
    public class GetAsyncTests
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

        #region GetAllAsync Tests

        [TestMethod]
        public async Task GetAllAsync_DebeRetornarListaDeDetalles_CuandoExistenDatos()
        {
            // Arrange
            var detalles = new List<DetalleReserva>
            {
                new DetalleReserva { ID = Guid.NewGuid().ToByteArray(), Reserva_ID = Guid.NewGuid().ToByteArray(), Habitacion_ID = Guid.NewGuid().ToByteArray(), Huesped_ID = Guid.NewGuid().ToByteArray() },
                new DetalleReserva { ID = Guid.NewGuid().ToByteArray(), Reserva_ID = Guid.NewGuid().ToByteArray(), Habitacion_ID = Guid.NewGuid().ToByteArray(), Huesped_ID = Guid.NewGuid().ToByteArray() }
            };
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(detalles);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsInstanceOfType(result, typeof(List<DetalleReservaDTO>));
        }

        [TestMethod]
        public async Task GetAllAsync_DebeRetornarListaVacia_CuandoNoExistenDatos()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<DetalleReserva>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region GetByIdAsync Tests

        [TestMethod]
        public async Task GetByIdAsync_DebeRetornarDetalle_CuandoIdExiste()
        {
            // Arrange
            var detalleId = Guid.NewGuid();
            var reservaId = Guid.NewGuid();
            var habitacionId = Guid.NewGuid();
            var huespedId = Guid.NewGuid();

            var detalle = new DetalleReserva
            {
                ID = detalleId.ToByteArray(),
                Reserva_ID = reservaId.ToByteArray(),
                Habitacion_ID = habitacionId.ToByteArray(),
                Huesped_ID = huespedId.ToByteArray()
    
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(detalleId.ToByteArray()))
                           .ReturnsAsync(detalle);

            // Act
            var result = await _service.GetByIdAsync(detalleId.ToString());

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(detalleId.ToString(), result.ID);
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public async Task GetByIdAsync_DebeLanzarNotFoundException_CuandoIdNoExiste()
        {
            // Arrange
            var detalleId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<byte[]>())).ReturnsAsync((DetalleReserva)null);

            // Act
            await _service.GetByIdAsync(detalleId.ToString());

            // Assert - La excepción es esperada
        }

        #endregion

        #region GetByReservaIdAsync Tests

        [TestMethod]
        public async Task GetByReservaIdAsync_DebeRetornarListaDeDetalles_CuandoReservaIdExiste()
        {
            // Arrange
            var reservaId = Guid.NewGuid();
            var habitacionId = Guid.NewGuid();
            var huespedId = Guid.NewGuid();

            var detalles = new List<DetalleReserva>
    {
        new DetalleReserva
        {
            ID = Guid.NewGuid().ToByteArray(),
            Reserva_ID = reservaId.ToByteArray(),
            Habitacion_ID = habitacionId.ToByteArray(),
            Huesped_ID = huespedId.ToByteArray()
        },
        new DetalleReserva
        {
            ID = Guid.NewGuid().ToByteArray(),
            Reserva_ID = reservaId.ToByteArray(),
            Habitacion_ID = habitacionId.ToByteArray(),
            Huesped_ID = huespedId.ToByteArray()
        }
    };

            _mockRepository.Setup(repo => repo.GetByReservaIdAsync(reservaId.ToByteArray())).ReturnsAsync(detalles);

            // Act
            var result = await _service.GetByReservaIdAsync(reservaId.ToString());

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(d => d.Reserva_ID == reservaId.ToString()));
        }

        [TestMethod]
        public async Task GetByReservaIdAsync_DebeRetornarListaVacia_CuandoReservaIdNoTieneDetalles()
        {
            // Arrange
            var reservaId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByReservaIdAsync(reservaId.ToByteArray())).ReturnsAsync(new List<DetalleReserva>());

            // Act
            var result = await _service.GetByReservaIdAsync(reservaId.ToString());

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        #endregion
    }
}