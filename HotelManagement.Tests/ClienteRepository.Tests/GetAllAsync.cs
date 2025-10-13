using Microsoft.VisualStudio.TestTools.UnitTesting;
using HotelManagement.Models;
using HotelManagement.Repositories;
using HotelManagement.Datos.Config;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// Agrega la siguiente directiva using para habilitar UseInMemoryDatabase
using Microsoft.EntityFrameworkCore.InMemory; // <-- FIX: Importa el paquete de extensi�n

namespace HotelManagement.Tests.Repository
{
    [TestClass]
    public class ClienteRepository_GetAllAsync_Tests
    {
        // M�todo auxiliar para crear un contexto de base de datos en memoria
        private HotelDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<HotelDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) // Base de datos �nica para cada prueba
                .Options;
            return new HotelDbContext(options);
        }

        [TestMethod]
        public async Task GetAllAsync_DebeDevolverListaDeClientes_CuandoExistenClientes()
        {
            // Arrange: Configuraci�n del escenario de prueba
            var dbContext = GetInMemoryDbContext();

            // A�adimos datos de prueba a nuestra base de datos en memoria
            var clientesDePrueba = new List<Cliente>
            {
                new Cliente { ID = System.Text.Encoding.UTF8.GetBytes("1"), Razon_Social = "Juan", NIT = "Perez", Email = "juan@example.com" },
                new Cliente { ID = System.Text.Encoding.UTF8.GetBytes("2"), Razon_Social = "Ana", NIT = "Gomez", Email = "ana@example.com"}
            };
            dbContext.Clientes.AddRange(clientesDePrueba);
            await dbContext.SaveChangesAsync();

            var repository = new ClienteRepository(dbContext);

            // Act: Ejecuci�n del m�todo que queremos probar
            var resultado = await repository.GetAllAsync();

            // Assert: Verificaci�n de los resultados
            Assert.IsNotNull(resultado);
            Assert.AreEqual(2, resultado.Count);
            Assert.IsTrue(resultado.Any(c => c.Razon_Social == "Juan"));
            Assert.IsTrue(resultado.Any(c => c.Razon_Social == "Ana"));
        }

        [TestMethod]
        public async Task GetAllAsync_DebeDevolverListaVacia_CuandoNoExistenClientes()
        {
            // Arrange: Configuraci�n del escenario de prueba
            // En este caso, no a�adimos datos al contexto para simular una tabla vac�a.
            var dbContext = GetInMemoryDbContext();
            var repository = new ClienteRepository(dbContext);

            // Act: Ejecuci�n del m�todo que queremos probar
            var resultado = await repository.GetAllAsync();

            // Assert: Verificaci�n de los resultados
            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Count);
        }
    }
}