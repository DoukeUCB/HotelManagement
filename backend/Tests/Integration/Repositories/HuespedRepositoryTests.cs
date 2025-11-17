using Xunit;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Datos.Config;
using HotelManagement.Models;
using HotelManagement.Repositories;

namespace HotelManagement.Tests.Integration.Repositories
{
    /// <summary>
    /// Pruebas de integración para HuespedRepository - Persistencia de Datos
    /// Secuencia: CREATE → SELECT → UPDATE → SELECT → DELETE → SELECT
    /// </summary>
    public class HuespedRepositoryTests : IDisposable
    {
        private readonly HotelDbContext _context;
        private readonly HuespedRepository _repository;

        public HuespedRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<HotelDbContext>()
                .UseInMemoryDatabase(databaseName: $"HuespedTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new HotelDbContext(options);
            _repository = new HuespedRepository(_context);
        }

        #region Happy Path Tests

        [Fact]
        public async Task HappyPath_CompleteFlow_HuespedCRUDOperations()
        {
            // ============================================================
            // PASO 1: CREATE - Insertar nuevo huésped
            // ============================================================
            var huespedId = Guid.NewGuid().ToByteArray();
            var huesped = new Huesped
            {
                ID = huespedId,
                Nombre = "JUAN CARLOS",
                Apellido = "PÉREZ",
                Segundo_Apellido = "GARCÍA",
                Documento_Identidad = "1234567",
                Telefono = "77123456",
                Fecha_Nacimiento = new DateTime(1990, 5, 15),
                Activo = true,
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };

            var createdHuesped = await _repository.CreateAsync(huesped);

            // Verificaciones POST CREATE
            Assert.NotNull(createdHuesped);
            Assert.Equal("JUAN CARLOS", createdHuesped.Nombre);
            Assert.Equal("PÉREZ", createdHuesped.Apellido);
            Assert.Equal("GARCÍA", createdHuesped.Segundo_Apellido);
            Assert.Equal("1234567", createdHuesped.Documento_Identidad);
            Assert.Equal("77123456", createdHuesped.Telefono);
            Assert.Equal(new DateTime(1990, 5, 15), createdHuesped.Fecha_Nacimiento);
            Assert.True(createdHuesped.Activo);
            Assert.True(createdHuesped.ID.SequenceEqual(huespedId));

            // ============================================================
            // PASO 2: SELECT - Recuperar huésped por ID
            // ============================================================
            var retrievedHuesped = await _repository.GetByIdAsync(huespedId);

            // Verificaciones POST SELECT
            Assert.NotNull(retrievedHuesped);
            Assert.Equal("JUAN CARLOS", retrievedHuesped.Nombre);
            Assert.Equal("PÉREZ", retrievedHuesped.Apellido);
            Assert.Equal("GARCÍA", retrievedHuesped.Segundo_Apellido);
            Assert.Equal("1234567", retrievedHuesped.Documento_Identidad);
            Assert.Equal("77123456", retrievedHuesped.Telefono);
            Assert.Equal(new DateTime(1990, 5, 15), retrievedHuesped.Fecha_Nacimiento);
            Assert.True(retrievedHuesped.Activo);

            // ============================================================
            // PASO 3: UPDATE - Actualizar datos del huésped
            // ============================================================
            retrievedHuesped.Nombre = "JUAN PABLO";
            retrievedHuesped.Segundo_Apellido = "RODRÍGUEZ";
            retrievedHuesped.Documento_Identidad = "7654321";
            retrievedHuesped.Telefono = "70987654";
            retrievedHuesped.Fecha_Actualizacion = DateTime.Now;

            var updatedHuesped = await _repository.UpdateAsync(retrievedHuesped);

            // Verificaciones POST UPDATE
            Assert.NotNull(updatedHuesped);
            Assert.Equal("JUAN PABLO", updatedHuesped.Nombre);
            Assert.Equal("RODRÍGUEZ", updatedHuesped.Segundo_Apellido);
            Assert.Equal("7654321", updatedHuesped.Documento_Identidad);
            Assert.Equal("70987654", updatedHuesped.Telefono);
            Assert.True(updatedHuesped.Fecha_Actualizacion > updatedHuesped.Fecha_Creacion);

            // ============================================================
            // PASO 4: SELECT - Verificar cambios persistidos
            // ============================================================
            var verifiedHuesped = await _repository.GetByIdAsync(huespedId);

            // Verificaciones POST UPDATE SELECT
            Assert.NotNull(verifiedHuesped);
            Assert.Equal("JUAN PABLO", verifiedHuesped.Nombre);
            Assert.Equal("RODRÍGUEZ", verifiedHuesped.Segundo_Apellido);
            Assert.Equal("7654321", verifiedHuesped.Documento_Identidad);
            Assert.Equal("70987654", verifiedHuesped.Telefono);

            // ============================================================
            // PASO 5: DELETE - Eliminar huésped
            // ============================================================
            var deleteResult = await _repository.DeleteAsync(huespedId);

            // Verificaciones POST DELETE
            Assert.True(deleteResult);

            // ============================================================
            // PASO 6: SELECT - Verificar eliminación
            // ============================================================
            var deletedHuesped = await _repository.GetByIdAsync(huespedId);

            // Verificaciones POST DELETE SELECT
            Assert.Null(deletedHuesped);
        }

        [Fact]
        public async Task HappyPath_CREATE_HuespedSinSegundoApellido_Success()
        {
            // Arrange
            var huespedId = Guid.NewGuid().ToByteArray();
            var huesped = new Huesped
            {
                ID = huespedId,
                Nombre = "MARÍA",
                Apellido = "LÓPEZ",
                Segundo_Apellido = null, // Sin segundo apellido
                Documento_Identidad = "9876543",
                Telefono = null, // Sin teléfono
                Fecha_Nacimiento = null, // Sin fecha de nacimiento
                Activo = true,
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };

            // Act
            var createdHuesped = await _repository.CreateAsync(huesped);

            // Assert
            Assert.NotNull(createdHuesped);
            Assert.Equal("MARÍA", createdHuesped.Nombre);
            Assert.Equal("LÓPEZ", createdHuesped.Apellido);
            Assert.Null(createdHuesped.Segundo_Apellido);
            Assert.Null(createdHuesped.Telefono);
            Assert.Null(createdHuesped.Fecha_Nacimiento);
        }

        #endregion

        #region Unhappy Path Tests

        [Fact]
        public async Task UnhappyPath_SELECT_HuespedInexistente_ReturnsNull()
        {
            // Arrange
            var idInexistente = Guid.NewGuid().ToByteArray();

            // Act
            var huesped = await _repository.GetByIdAsync(idInexistente);

            // Assert
            Assert.Null(huesped);
        }

        [Fact]
        public async Task UnhappyPath_DELETE_HuespedInexistente_ReturnsFalse()
        {
            // Arrange
            var idInexistente = Guid.NewGuid().ToByteArray();

            // Act
            var result = await _repository.DeleteAsync(idInexistente);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UnhappyPath_CREATE_DocumentoDuplicado_AllowedInMemory()
        {
            // Arrange - Crear primer huésped
            var huesped1 = new Huesped
            {
                ID = Guid.NewGuid().ToByteArray(),
                Nombre = "PEDRO",
                Apellido = "GÓMEZ",
                Documento_Identidad = "9999999",
                Activo = true,
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };
            await _repository.CreateAsync(huesped1);

            // Act - Intentar crear segundo huésped con mismo documento
            var huesped2 = new Huesped
            {
                ID = Guid.NewGuid().ToByteArray(),
                Nombre = "ANA",
                Apellido = "MARTÍNEZ",
                Documento_Identidad = "9999999", // Documento duplicado
                Activo = true,
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };

            await _repository.CreateAsync(huesped2);

            // Assert - En InMemoryDatabase se permite, en producción fallaría
            var allHuespedes = await _repository.GetAllAsync();
            var huespedesConMismoDoc = allHuespedes
                .Where(h => h.Documento_Identidad == "9999999")
                .ToList();
            
            Assert.Equal(2, huespedesConMismoDoc.Count);
        }

        [Fact]
        public async Task UnhappyPath_CREATE_NombreConNumeros_AllowedInMemory()
        {
            // Arrange
            var huesped = new Huesped
            {
                ID = Guid.NewGuid().ToByteArray(),
                Nombre = "JUAN123", // Nombre con números
                Apellido = "PÉREZ",
                Documento_Identidad = "5555555",
                Activo = true,
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };

            // Act
            await _repository.CreateAsync(huesped);

            // Assert - En InMemoryDatabase se permite
            var created = await _repository.GetByIdAsync(huesped.ID);
            Assert.NotNull(created);
            Assert.Equal("JUAN123", created.Nombre);
            
            // Nota: Los validadores deberían prevenir esto antes de llegar al repositorio
        }

        [Fact]
        public async Task UnhappyPath_CREATE_DocumentoMuyCorto_AllowedInMemory()
        {
            // Arrange
            var huesped = new Huesped
            {
                ID = Guid.NewGuid().ToByteArray(),
                Nombre = "CARLOS",
                Apellido = "RUIZ",
                Documento_Identidad = "123", // Documento muy corto (< 5 caracteres)
                Activo = true,
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };

            // Act
            await _repository.CreateAsync(huesped);

            // Assert - En InMemoryDatabase se permite
            var created = await _repository.GetByIdAsync(huesped.ID);
            Assert.NotNull(created);
            Assert.Equal("123", created.Documento_Identidad);
            
            // Nota: Los validadores deberían prevenir esto antes de llegar al repositorio
        }

        [Fact]
        public async Task UnhappyPath_CREATE_FechaNacimientoFutura_AllowedInMemory()
        {
            // Arrange
            var huesped = new Huesped
            {
                ID = Guid.NewGuid().ToByteArray(),
                Nombre = "FUTURO",
                Apellido = "VIAJERO",
                Documento_Identidad = "8888888",
                Fecha_Nacimiento = DateTime.Now.AddYears(10), // Fecha futura
                Activo = true,
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };

            // Act
            await _repository.CreateAsync(huesped);

            // Assert - En InMemoryDatabase se permite
            var created = await _repository.GetByIdAsync(huesped.ID);
            Assert.NotNull(created);
            Assert.True(created.Fecha_Nacimiento > DateTime.Now);
            
            // Nota: Los validadores deberían prevenir esto antes de llegar al repositorio
        }

        [Fact]
        public async Task UnhappyPath_GetAll_ConMultiplesHuespedes_ReturnsAll()
        {
            // Arrange - Crear múltiples huéspedes
            var huespedes = new[]
            {
                new Huesped { ID = Guid.NewGuid().ToByteArray(), Nombre = "LUIS", Apellido = "FERNÁNDEZ", Documento_Identidad = "1111111", Activo = true, Fecha_Creacion = DateTime.Now, Fecha_Actualizacion = DateTime.Now },
                new Huesped { ID = Guid.NewGuid().ToByteArray(), Nombre = "SOFIA", Apellido = "TORRES", Documento_Identidad = "2222222", Activo = false, Fecha_Creacion = DateTime.Now, Fecha_Actualizacion = DateTime.Now },
                new Huesped { ID = Guid.NewGuid().ToByteArray(), Nombre = "DIEGO", Apellido = "MORALES", Documento_Identidad = "3333333", Activo = true, Fecha_Creacion = DateTime.Now, Fecha_Actualizacion = DateTime.Now }
            };

            foreach (var h in huespedes)
            {
                await _repository.CreateAsync(h);
            }

            // Act
            var allHuespedes = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(3, allHuespedes.Count);
            Assert.Contains(allHuespedes, h => h.Nombre == "LUIS");
            Assert.Contains(allHuespedes, h => h.Nombre == "SOFIA");
            Assert.Contains(allHuespedes, h => h.Nombre == "DIEGO");
            Assert.Contains(allHuespedes, h => h.Activo == false); // Verificar huésped inactivo
        }

        [Fact]
        public async Task UnhappyPath_UPDATE_HuespedInexistente_NoEffect()
        {
            // Arrange
            var huespedInexistente = new Huesped
            {
                ID = Guid.NewGuid().ToByteArray(),
                Nombre = "FANTASMA",
                Apellido = "INEXISTENTE",
                Documento_Identidad = "0000000",
                Activo = true,
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };

            // Act & Assert - InMemoryDatabase lanza excepción al actualizar entidad inexistente
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
            {
                await _repository.UpdateAsync(huespedInexistente);
            });
        }

        [Fact]
        public async Task UnhappyPath_GetByDocumento_HuespedInexistente_ReturnsNull()
        {
            // Arrange
            var documentoInexistente = "9999999999";

            // Act
            var huesped = await _repository.GetByDocumentoAsync(documentoInexistente);

            // Assert
            Assert.Null(huesped);
        }

        [Fact]
        public async Task HappyPath_GetByDocumento_HuespedExistente_ReturnsHuesped()
        {
            // Arrange
            var huesped = new Huesped
            {
                ID = Guid.NewGuid().ToByteArray(),
                Nombre = "ENCONTRADO",
                Apellido = "POR DOCUMENTO",
                Documento_Identidad = "7777777",
                Activo = true,
                Fecha_Creacion = DateTime.Now,
                Fecha_Actualizacion = DateTime.Now
            };
            await _repository.CreateAsync(huesped);

            // Act
            var found = await _repository.GetByDocumentoAsync("7777777");

            // Assert
            Assert.NotNull(found);
            Assert.Equal("ENCONTRADO", found.Nombre);
            Assert.Equal("7777777", found.Documento_Identidad);
        }

        #endregion

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
