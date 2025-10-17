// Importa los namespaces necesarios
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HotelManagement.Services;
using Moq;
using HotelManagement.DTOs;
using HotelManagement.Models;
using HotelManagement.Repositories;
using HotelManagement.Aplicacion.Validators;
using HotelManagement.Aplicacion.Exceptions; // Aseg�rate de que esta excepci�n exista
using System;
using System.Threading.Tasks;

namespace HotelManagement.Tests.Services
{
    [TestClass]
    public class CreateAsyncTests
    {
        // Declaramos los mocks y el servicio que vamos a probar
        private Mock<IClienteRepository> _mockRepository;
        private Mock<IClienteValidator> _mockValidator;
        private ClienteService _clienteService;

        [TestInitialize] // Este m�todo se ejecuta antes de cada prueba
        public void Setup()
        {
            // Creamos nuevas instancias de los mocks para cada prueba, asegurando el aislamiento
            _mockRepository = new Mock<IClienteRepository>();
            _mockValidator = new Mock<IClienteValidator>();

            // Instanciamos el servicio con los mocks
            _clienteService = new ClienteService(_mockRepository.Object, _mockValidator.Object);
        }

        [TestMethod]
        [TestCategory("HappyPath")]
        public async Task CreateAsync_WithValidData_ShouldReturnCreatedClienteDTO()
        {
            // --- ARRANGE ---

            // 1. Definimos el DTO de entrada para la prueba
            var clienteCreateDto = new ClienteCreateDTO
            {
                Razon_Social = "Cliente de Prueba S.A.",
                NIT = "123456789",
                Email = "contacto@clienteprueba.com"
            };

            // 2. Simulamos el comportamiento del validador. En este caso, no hace nada (pasa la validaci�n).
            _mockValidator
                .Setup(v => v.ValidateCreateAsync(clienteCreateDto))
                .Returns(Task.CompletedTask); // Simula un m�todo async que completa exitosamente

            // 3. Simulamos el comportamiento del repositorio. Cuando se llame a CreateAsync,
            // devolver� un objeto Cliente con los datos esperados y un ID generado.
            _mockRepository
                .Setup(r => r.CreateAsync(It.IsAny<Cliente>())) // Acepta cualquier objeto Cliente como entrada
                .ReturnsAsync((Cliente cliente) => cliente); // Retorna el mismo cliente que recibi�

            // --- ACT ---

            // 4. Ejecutamos el m�todo que estamos probando
            var resultDto = await _clienteService.CreateAsync(clienteCreateDto);

            // --- ASSERT ---

            // 5. Verificamos que los resultados sean los esperados
            Assert.IsNotNull(resultDto, "El DTO resultante no deber�a ser nulo.");
            Assert.AreEqual(clienteCreateDto.Razon_Social, resultDto.Razon_Social, "La Raz�n Social no coincide.");
            Assert.AreEqual(clienteCreateDto.NIT, resultDto.NIT, "El NIT no coincide.");
            Assert.AreEqual(clienteCreateDto.Email, resultDto.Email, "El Email no coincide.");

            // 6. Verificamos que los m�todos de los mocks fueron llamados como se esperaba
            _mockValidator.Verify(v => v.ValidateCreateAsync(clienteCreateDto), Times.Once, "El validador no fue llamado exactamente una vez.");
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Cliente>()), Times.Once, "El repositorio no fue llamado exactamente una vez.");
        }

        [TestMethod]
        [TestCategory("ExceptionPath")]
        public async Task CreateAsync_WhenValidatorThrowsException_ShouldPropagateException()
        {
            // --- ARRANGE ---

            // 1. Definimos un DTO de entrada (puede ser inv�lido o no, lo importante es el mock)
            var invalidClienteCreateDto = new ClienteCreateDTO { NIT = "123" }; // Datos inv�lidos
            var expectedExceptionMessage = "El NIT ya existe.";

            // 2. Simulamos que el validador lanza una excepci�n cuando es llamado.
            _mockValidator
                .Setup(v => v.ValidateCreateAsync(invalidClienteCreateDto))
                .ThrowsAsync(new ValidationException(expectedExceptionMessage)); // Simula que la validaci�n falla

            // --- ACT & ASSERT ---

            // 3. Usamos Assert.ThrowsExceptionAsync para verificar que se lanza la excepci�n esperada
            var exception = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _clienteService.CreateAsync(invalidClienteCreateDto),
                "Se esperaba una excepci�n de tipo ValidationException, pero no fue lanzada."
            );

            // 4. (Opcional) Verificamos que el mensaje de la excepci�n sea el correcto
            Assert.AreEqual(expectedExceptionMessage, exception.Message);

            // 5. Verificamos que el m�todo del repositorio NUNCA fue llamado, ya que la validaci�n fall� antes.
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Cliente>()), Times.Never, "El repositorio no deber�a haber sido llamado si la validaci�n fall�.");

            // 6. Verificamos que el validador s� fue llamado.
            _mockValidator.Verify(v => v.ValidateCreateAsync(invalidClienteCreateDto), Times.Once, "El validador debi� ser llamado.");
        }
    }
}