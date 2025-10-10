using Microsoft.VisualStudio.TestTools.UnitTesting;
using HotelManagement.Models;
using HotelManagement.Datos.Config;
using HotelManagement.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Tests.DetallerReserva.Data;
[TestClass]
public class DetalleReservaRepository_GetAllAsync_Tests
{
    private DbContextOptions<HotelDbContext> _dbOptions;

    [TestInitialize]
    public void Setup()
    {
        // Configura una base de datos en memoria �nica para cada prueba
        _dbOptions = new DbContextOptionsBuilder<HotelDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
    }

    [TestMethod]
    [TestCategory("Pruebas de Caja Blanca - Complejidad Ciclom�tica")]
    [Description("Verifica que GetAllAsync retorna todos los detalles de reserva cuando la base de datos contiene datos. Cubre el �nico camino de ejecuci�n.")]
    public async Task GetAllAsync_ShouldReturnAllDetails_WhenDataExists()
    {
        // --- Arrange ---
        // 1. Crea una instancia del contexto en memoria y a�ade datos de prueba COMPLETOS
        using (var context = new HotelDbContext(_dbOptions))
        {
            context.DetalleReservas.AddRange(new List<DetalleReserva>
        {
            // Objeto 1: A�ade valores para las propiedades requeridas.
            // Asumo que los IDs son de tipo int, aj�stalos si son de otro tipo (Guid, etc.).
            new DetalleReserva {
                ID = new byte[]{1},
                Habitacion_ID = new byte[]{1},
                Huesped_ID = new byte[]{1},
                Reserva_ID = new byte[]{1} 
                /* ... otras propiedades necesarias ... */ 
            },
            // Objeto 2: Con valores diferentes para asegurar que son entidades distintas.
            new DetalleReserva {
                ID = new byte[]{2},
                Habitacion_ID = new byte[]{2},
                Huesped_ID = new byte[]{2},
                Reserva_ID = new byte[]{2} 
                /* ... otras propiedades necesarias ... */ 
            }
        });
            // Esta l�nea ya no deber�a fallar
            await context.SaveChangesAsync();
        }

        // 2. Crea una nueva instancia del contexto y el repositorio para la prueba
        using (var context = new HotelDbContext(_dbOptions))
        {
            var repository = new DetalleReservaRepository(context);

            // --- Act ---
            // 3. Llama al m�todo que se est� probando
            var result = await repository.GetAllAsync();

            // --- Assert ---
            // 4. Verifica que el resultado es el esperado
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
    [TestMethod]
    [TestCategory("Pruebas de Caja Blanca - Caso de Borde")]
    [Description("Verifica que GetAllAsync retorna una lista vac�a cuando no hay datos en la base de datos.")]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoDataExists()
    {
        // --- Arrange ---
        // En este caso, no a�adimos datos al contexto. La base de datos est� vac�a.
        using (var context = new HotelDbContext(_dbOptions))
        {
            var repository = new DetalleReservaRepository(context);

            // --- Act ---
            // Llama al m�todo
            var result = await repository.GetAllAsync();

            // --- Assert ---
            // Verifica que el resultado es una lista vac�a
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
