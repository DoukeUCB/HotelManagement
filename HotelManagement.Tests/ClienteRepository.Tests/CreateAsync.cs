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
        // M�todo privado para configurar la base de datos en memoria para cada prueba
        private HotelManagement.Datos.Config.HotelDbContext GetInMemoryDbContext()
        {
            // Usar un nombre de base de datos �nico para cada prueba para garantizar el aislamiento
            var options = new DbContextOptionsBuilder<HotelManagement.Datos.Config.HotelDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new HotelManagement.Datos.Config.HotelDbContext(options);
        }

        /*
         * PRUEBA PARA EL M�TODO CreateAsync
         * * An�lisis de Caja Blanca:
         * - Complejidad Ciclom�tica: 1. El m�todo no tiene bifurcaciones ni bucles. 
         * Sigue una �nica ruta de ejecuci�n:
         * 1. _context.Clientes.Add(cliente);
         * 2. await _context.SaveChangesAsync();
         * 3. return cliente;
         * * Caso de Prueba:
         * - Objetivo: Verificar que cuando se llama a CreateAsync con un objeto Cliente v�lido,
         * este se a�ade a la base de datos y se devuelve correctamente.
         * - Cobertura: Este �nico caso de prueba cubre el 100% del c�digo del m�todo.
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
            // Llamar al m�todo que queremos probar.
            var result = await repository.CreateAsync(newCliente);

            // --- ASSERT ---
            // 1. Verificar que el m�todo devolvi� el mismo cliente.
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