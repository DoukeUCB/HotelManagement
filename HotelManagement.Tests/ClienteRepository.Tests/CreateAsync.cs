using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Datos.Config;
using HotelManagement.Models;
using HotelManagement.Repositories;
using System.Threading.Tasks;
using System.Linq;

namespace HotelManagement.Tests.Repository
{
    [TestClass]
    public class ClienteRepository_CreateAsync_Tests
    {
        // Método privado para configurar la base de datos en memoria para cada prueba
        private HotelManagement.Datos.Config.HotelDbContext GetInMemoryDbContext()
        {
            // Usar un nombre de base de datos único para cada prueba para garantizar el aislamiento
            var options = new DbContextOptionsBuilder<HotelManagement.Datos.Config.HotelDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new HotelManagement.Datos.Config.HotelDbContext(options);
        }

        /*
         * PRUEBA PARA EL MÉTODO CreateAsync
         * * Análisis de Caja Blanca:
         * - Complejidad Ciclomática: 1. El método no tiene bifurcaciones ni bucles. 
         * Sigue una única ruta de ejecución:
         * 1. _context.Clientes.Add(cliente);
         * 2. await _context.SaveChangesAsync();
         * 3. return cliente;
         * * Caso de Prueba:
         * - Objetivo: Verificar que cuando se llama a CreateAsync con un objeto Cliente válido,
         * este se añade a la base de datos y se devuelve correctamente.
         * - Cobertura: Este único caso de prueba cubre el 100% del código del método.
        */
        [TestMethod]
        public async Task CreateAsync_ShouldAddClienteAndSaveChanges()
        {
            // --- ARRANGE ---
            // 1. Configurar el DbContext en memoria.
            await using var context = GetInMemoryDbContext();

            // 2. Crear una instancia del repositorio con el contexto en memoria.
            var repository = new ClienteRepository(context);

            // 3. Crear un nuevo objeto Cliente para la prueba.
            var newCliente = new Cliente
            {
                ID = System.Text.Encoding.UTF8.GetBytes("1"), // Simulamos un ID
                Razon_Social = "Juan",
                Email = "juan.perez@example.com",
                NIT = "123456789"
            };

            // --- ACT ---
            // Llamar al método que queremos probar.
            var result = await repository.CreateAsync(newCliente);

            // --- ASSERT ---
            // 1. Verificar que el método devolvió el mismo cliente.
            Assert.IsNotNull(result);
            Assert.AreEqual("Juan", result.Razon_Social);

            // 2. Verificar que el cliente fue realmente guardado en la base de datos.
            // Se consulta directamente el contexto para confirmar que hay 1 cliente.
            Assert.AreEqual(1, await context.Clientes.CountAsync());

            // 3. Obtener el cliente de la "base de datos" y verificar sus propiedades.
            var clienteInDb = await context.Clientes.FirstOrDefaultAsync();
            Assert.IsNotNull(clienteInDb);
            Assert.AreEqual("juan.perez@example.com", clienteInDb.Email);
        }
    }
}