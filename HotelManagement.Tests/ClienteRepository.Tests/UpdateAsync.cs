// File: HotelManagement.Tests/Repositories/ClienteRepositoryTests.cs

using Microsoft.EntityFrameworkCore;
using HotelManagement.Models;
using HotelManagement.Repositories;
using HotelManagement.Datos.Config;

namespace HotelManagement.Tests.Repository
{
    [TestClass]
    public class UpdateASYNCTests
    {
        private DbContextOptions<HotelDbContext> _dbContextOptions;

        // Este método se ejecuta antes de cada prueba para configurar la base de datos en memoria.
        [TestInitialize]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<HotelDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Base de datos única para cada prueba
                .Options;
        }

        #region Pruebas para UpdateAsync

        /// <summary>
        /// Caso de prueba 1 (Único camino):
        /// Objetivo: Verificar que cuando se pasa un cliente válido, se actualiza en la base de datos
        /// y se devuelve el cliente actualizado.
        /// </summary>
        [TestMethod]
        public async Task UpdateAsync_WhenClienteExists_ShouldUpdateAndReturnCliente()
        {
            // --- Arrange (Preparar) ---

            // 1. Crear el cliente inicial y los datos actualizados.
            var clientId = Guid.NewGuid().ToByteArray();
            var initialCliente = new Cliente
            {
                ID = clientId,
                Razon_Social = "Juan",
                Email = "juan.perez@test.com",
                NIT = "12345678"
            };

            var updatedClienteData = new Cliente
            {
                ID = clientId, // Mismo ID
                Razon_Social = "Juan Alberto", // Nombre actualizado
                Email = "juan.perez@test.com",
                NIT = "87654321" // Teléfono actualizado
            };

            // 2. Configurar el contexto de la base de datos en memoria.
            //    Usamos 'await using' para asegurar que el contexto se deseche correctamente.
            await using (var context = new HotelDbContext(_dbContextOptions))
            {
                // Agregamos el cliente inicial a la "base de datos".
                context.Clientes.Add(initialCliente);
                await context.SaveChangesAsync();
            }

            // --- Act (Actuar) ---
            Cliente result;
            await using (var context = new HotelDbContext(_dbContextOptions))
            {
                // Creamos una instancia del repositorio con nuestro contexto de prueba.
                var repository = new ClienteRepository(context);

                // Ejecutamos el método que queremos probar.
                result = await repository.UpdateAsync(updatedClienteData);
            }

            // --- Assert (Afirmar) ---

            // 1. Verificar que el cliente devuelto no es nulo y tiene los datos actualizados.
            Assert.IsNotNull(result);
            Assert.AreEqual("Juan Alberto", result.Razon_Social);
            Assert.AreEqual("87654321", result.NIT);

            // 2. Verificar que los datos se persistieron correctamente en la base de datos.
            //    Creamos un nuevo contexto para asegurar que estamos leyendo desde el "almacenamiento" y no desde la caché.
            await using (var context = new HotelDbContext(_dbContextOptions))
            {
                var clienteFromDb = await context.Clientes.FindAsync(clientId);
                Assert.IsNotNull(clienteFromDb);
                Assert.AreEqual("Juan Alberto", clienteFromDb.Razon_Social);
                Assert.AreEqual("87654321", clienteFromDb.NIT);
            }
        }

        #endregion
    }
}