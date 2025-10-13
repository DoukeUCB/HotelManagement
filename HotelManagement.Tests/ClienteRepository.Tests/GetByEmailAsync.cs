using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Datos.Config;
using HotelManagement.Models;
using HotelManagement.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HotelManagement.Tests.Repository;

[TestClass]
public class GetByEmailAsyncTests
{
    private DbContextOptions<HotelDbContext> _options;

    // Este método se ejecuta antes de cada prueba para configurar la base de datos en memoria.
    [TestInitialize]
    public void Setup()
    {
        // Configura la base de datos en memoria con un nombre único para cada prueba
        _options = new DbContextOptionsBuilder<HotelDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
    }

    #region Pruebas para GetByEmailAsync

    /// <summary>
    /// Caso de Prueba 1: Verifica que la función devuelve el objeto Cliente
    /// cuando el email proporcionado existe en la base de datos.
    /// Corresponde al "camino feliz" o la ruta donde se encuentra una coincidencia.
    /// </summary>
    [TestMethod]
    public async Task GetByEmailAsync_ShouldReturnCliente_WhenEmailExists()
    {
        // Arrange (Preparar)
        var clienteExistente = new Cliente { ID = System.Text.Encoding.UTF8.GetBytes("1"), Razon_Social = "Juan Perez", Email = "juan@example.com", NIT= "12345678" };

        // Usamos un 'using' para asegurar que el contexto se deseche correctamente
        await using (var context = new HotelDbContext(_options))
        {
            await context.Clientes.AddAsync(clienteExistente);
            await context.SaveChangesAsync();
        }

        await using (var context = new HotelDbContext(_options))
        {
            var repository = new ClienteRepository(context);

            // Act (Actuar)
            var resultado = await repository.GetByEmailAsync("juan@example.com");

            // Assert (Afirmar)
            Assert.IsNotNull(resultado);
            Assert.AreEqual("Juan Perez", resultado.Razon_Social);
            Assert.AreEqual("juan@example.com", resultado.Email);
        }
    }

    /// <summary>
    /// Caso de Prueba 2: Verifica que la función devuelve null
    /// cuando el email proporcionado NO existe en la base de datos.
    /// Corresponde al camino donde no se encuentra ninguna coincidencia.
    /// </summary>
    [TestMethod]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
    {
        // Arrange (Preparar)
        // La base de datos está vacía en este caso, o podemos agregar datos que no coincidan.
        var otroCliente = new Cliente { ID = System.Text.Encoding.UTF8.GetBytes("1"), Razon_Social = "Ana Gomez", Email = "ana@example.com", NIT = "87654321" };

        await using (var context = new HotelDbContext(_options))
        {
            await context.Clientes.AddAsync(otroCliente);
            await context.SaveChangesAsync();
        }

        await using (var context = new HotelDbContext(_options))
        {
            var repository = new ClienteRepository(context);

            // Act (Actuar)
            var resultado = await repository.GetByEmailAsync("email.inexistente@example.com");

            // Assert (Afirmar)
            Assert.IsNull(resultado);
        }
    }

    #endregion
}