using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using HotelManagement.Controllers;
using HotelManagement.DTOs;
using HotelManagement.Services;

namespace HotelManagement.Tests.DetallerReserva.Controller;

[TestClass]
public class GetByIdTests
{
    private readonly Mock<IDetalleReservaService> _mockService;
    private readonly Mock<ILogger<DetalleReservaController>> _mockLogger;
    private readonly DetalleReservaController _controller;

    public GetByIdTests()
    {
        _mockService = new Mock<IDetalleReservaService>();
        _mockLogger = new Mock<ILogger<DetalleReservaController>>();
        _controller = new DetalleReservaController(_mockService.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task GetById_ReturnsOkObjectResult_WhenDetalleReservaExists()
    {
        // Arrange
        var testId = "some-guid";
        var detalleReservaDto = new DetalleReservaDTO { ID = testId, /* ... otras propiedades ... */ };
        _mockService.Setup(service => service.GetByIdAsync(testId))
            .ReturnsAsync(detalleReservaDto);

        // Act
        var result = await _controller.GetById(testId);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var returnedDto = okResult.Value as DetalleReservaDTO;
        Assert.IsNotNull(returnedDto);
        Assert.AreEqual(testId, returnedDto.ID);
    }

    [TestMethod]
    public async Task GetById_ReturnsOkObjectResultWithNull_WhenDetalleReservaDoesNotExist()
    {
        // Arrange
        var testId = "non-existing-guid";
        _mockService.Setup(service => service.GetByIdAsync(testId))
            .ReturnsAsync((DetalleReservaDTO)null);

        // Act
        var result = await _controller.GetById(testId);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsNull(okResult.Value);
    }
}