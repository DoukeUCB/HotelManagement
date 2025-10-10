using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HotelManagement.Services;
using HotelManagement.Presentacion.Controllers;
using HotelManagement.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Tests.Controller;

[TestClass]
public class GetAllTests // Renombrado para mayor claridad
{
    private Mock<IClienteService> _mockClienteService;
    private ClienteController _controller;

    // Usamos TestInitialize para configurar el mock y el controlador antes de cada prueba
    [TestInitialize]
    public void Setup()
    {
        _mockClienteService = new Mock<IClienteService>();
        _controller = new ClienteController(_mockClienteService.Object);
    }

    [TestMethod]
    public async Task GetAllAsync_DeberiaDevolverOkConListaDeClientes_CuandoExistenClientes()
    {
        // Arrange (Organizar)
        var clientesDto = new List<ClienteDTO>
        {
            new ClienteDTO { ID = "1", Razon_Social = "Juan Perez", Email = "juan@example.com" },
            new ClienteDTO { ID = "2", Razon_Social = "Ana Lopez", Email = "ana@example.com" }
        };

        // Configuramos el mock para que devuelva la lista de clientes cuando se llame a GetAllAsync
        _mockClienteService.Setup(service => service.GetAllAsync()).ReturnsAsync(clientesDto);

        // Act (Actuar)
        var resultado = await _controller.GetAll();

        // Assert (Afirmar)
        // 1. Verificar que el resultado es de tipo OkObjectResult (un código 200 OK)
        var okResult = resultado as OkObjectResult;
        Assert.IsNotNull(okResult);

        // 2. Verificar que el valor dentro del resultado es del tipo esperado (lista de ClienteDTO)
        var listaResultado = okResult.Value as List<ClienteDTO>;
        Assert.IsNotNull(listaResultado);

        // 3. Verificar que la cantidad de elementos es la correcta
        Assert.AreEqual(2, listaResultado.Count);
    }

    [TestMethod]
    public async Task GetAllAsync_DeberiaDevolverOkConListaVacia_CuandoNoExistenClientes()
    {
        // Arrange (Organizar)
        // Configuramos el mock para que devuelva una lista vacía
        _mockClienteService.Setup(service => service.GetAllAsync()).ReturnsAsync(new List<ClienteDTO>());

        // Act (Actuar)
        var resultado = await _controller.GetAll();

        // Assert (Afirmar)
        // 1. Verificar que el resultado es de tipo OkObjectResult
        var okResult = resultado as OkObjectResult;
        Assert.IsNotNull(okResult);

        // 2. Verificar que el valor es una lista
        var listaResultado = okResult.Value as List<ClienteDTO>;
        Assert.IsNotNull(listaResultado);

        // 3. Verificar que la lista está vacía
        Assert.AreEqual(0, listaResultado.Count);
    }
}