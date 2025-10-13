using Microsoft.VisualStudio.TestTools.UnitTesting;
using HotelManagement.Aplicacion.Exceptions; // Asegúrate de que el namespace sea correcto
using System.Collections.Generic;
using System.Linq;

namespace HotelManagement.Tests.CustomException
{
    [TestClass]
    public class CustomExceptionsTests
    {
        [TestMethod]
        public void NotFoundException_Constructor_ShouldSetMessageCorrectly()
        {
            // Arrange
            var expectedMessage = "Recurso no encontrado.";

            // Act
            var exception = new NotFoundException(expectedMessage);

            // Assert
            Assert.IsNotNull(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOfType(exception, typeof(System.Exception));
        }

        [TestMethod]
        public void BadRequestException_Constructor_ShouldSetMessageCorrectly()
        {
            // Arrange
            var expectedMessage = "La solicitud es incorrecta.";

            // Act
            var exception = new BadRequestException(expectedMessage);

            // Assert
            Assert.IsNotNull(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOfType(exception, typeof(System.Exception));
        }

        [TestMethod]
        public void ConflictException_Constructor_ShouldSetMessageCorrectly()
        {
            // Arrange
            var expectedMessage = "Existe un conflicto con el recurso.";

            // Act
            var exception = new ConflictException(expectedMessage);

            // Assert
            Assert.IsNotNull(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOfType(exception, typeof(System.Exception));
        }

        [TestMethod]
        public void ValidationException_ConstructorWithString_ShouldSetMessageAndCreateErrorList()
        {
            // Arrange
            var errorMessage = "El campo 'Nombre' es obligatorio.";

            // Act
            var exception = new ValidationException(errorMessage);

            // Assert
            Assert.IsNotNull(exception);
            Assert.AreEqual(errorMessage, exception.Message); // El mensaje base de la excepción
            Assert.IsNotNull(exception.Errors);
            Assert.AreEqual(1, exception.Errors.Count);
            Assert.AreEqual(errorMessage, exception.Errors[0]);
        }

        [TestMethod]
        public void ValidationException_ConstructorWithList_ShouldSetErrorsAndDefaultMessage()
        {
            // Arrange
            var expectedErrors = new List<string>
            {
                "Error de validación 1.",
                "Error de validación 2."
            };
            var expectedBaseMessage = "Errores de validación";

            // Act
            var exception = new ValidationException(expectedErrors);

            // Assert
            Assert.IsNotNull(exception);
            Assert.AreEqual(expectedBaseMessage, exception.Message);
            Assert.IsNotNull(exception.Errors);
            Assert.AreEqual(2, exception.Errors.Count);
            // Comprueba que todos los errores esperados están en la lista de la excepción
            CollectionAssert.AreEquivalent(expectedErrors, exception.Errors);
        }
    }
}