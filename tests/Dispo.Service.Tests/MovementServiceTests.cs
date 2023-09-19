using Dispo.Domain.DTOs.RequestDTOs;
using Dispo.Domain.Entities;
using Dispo.Domain.Enums;
using Dispo.Domain.Exceptions;
using Dispo.Infrastructure.Repositories.Interfaces;
using Dispo.Service.Services;
using Dispo.Service.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dispo.Service.Tests
{
    public class MovementServiceTests
    {
        private MovementService _sut;
        private Mock<IMovementRepository> _movementRepositoryMock;
        private Mock<IProductService> _productServiceMock;
        private Mock<ILogger<MovementService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _movementRepositoryMock = new Mock<IMovementRepository>();
            _productServiceMock = new Mock<IProductService>();
            _loggerMock = new Mock<ILogger<MovementService>>();

            _sut = new MovementService(_movementRepositoryMock.Object, _productServiceMock.Object, null, null, null, null, _loggerMock.Object);
        }

        [Test]
        public async Task MoveProductAsync_ValidProductMovimentation_UpdateWarehouseQuantity()
        {
            // Arrange
            var productMovimentationDto = new ProductMovimentationDto(1, 1, 1, eMovementType.Output);

            _productServiceMock.Setup(s => s.ExistsByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(true);

            _movementRepositoryMock.Setup(s => s.CreateAsync(It.IsAny<Movement>()))
                .ReturnsAsync(true);

            //_productWarehouseQuantityServiceMock.Setup(s => s.UpdateProductWarehouseQuantityAsync(It.IsAny<ProductMovimentationDto>())).ReturnsAsync(true);

            // Act
            await _sut.MoveProductAsync(productMovimentationDto);

            // Assert
            //_productWarehouseQuantityServiceMock.Verify(v => v.UpdateProductWarehouseQuantityAsync(productMovimentationDto), Times.Once);
        }

        [Test]
        public async Task MoveProductAsync_ProductDoesntExists_DontUpdateWarehouseQuantityAndThrowException()
        {
            // Arrange
            var productMovimentationDto = new ProductMovimentationDto(1, 1, 1, eMovementType.Output);

            _productServiceMock.Setup(s => s.ExistsByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(false);

            // Act
            var result = Assert.ThrowsAsync<NotFoundException>(async () => await _sut.MoveProductAsync(productMovimentationDto));

            // Assert
            Assert.That(result.Message, Is.EqualTo(string.Format("Produto com o Id {0} n�o foi encontrado.", productMovimentationDto.ProductId)));
            //_productWarehouseQuantityServiceMock.Verify(v => v.UpdateProductWarehouseQuantityAsync(productMovimentationDto), Times.Never);
        }

        [Test]
        public async Task MoveProductAsync_MovementCouldntBeCreated_DontUpdateWarehouseQuantityAndThrowException()
        {
            // Arrange
            var productMovimentationDto = new ProductMovimentationDto(1, 1, 1, eMovementType.Output);

            _productServiceMock.Setup(s => s.ExistsByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(true);

            _movementRepositoryMock.Setup(s => s.CreateAsync(It.IsAny<Movement>()))
                .ReturnsAsync(false);

            // Act
            var result = Assert.ThrowsAsync<UnhandledException>(async () => await _sut.MoveProductAsync(productMovimentationDto));

            // Assert
            Assert.That(result.Message, Is.EqualTo("Movimenta��o n�o pode ser criada."));
            //_productWarehouseQuantityServiceMock.Verify(v => v.UpdateProductWarehouseQuantityAsync(productMovimentationDto), Times.Never);
        }

        [Test]
        public async Task MoveProductAsync_ProductWarehouseQuantityCoulntBeUpdated_DontUpdateWarehouseQuantityAndThrowException()
        {
            // Arrange
            var productMovimentationDto = new ProductMovimentationDto(1, 1, 1, eMovementType.Output);

            _productServiceMock.Setup(s => s.ExistsByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(true);

            _movementRepositoryMock.Setup(s => s.CreateAsync(It.IsAny<Movement>()))
                .ReturnsAsync(true);

            //_productWarehouseQuantityServiceMock.Setup(s => s.UpdateProductWarehouseQuantityAsync(It.IsAny<ProductMovimentationDto>())).ReturnsAsync(false);

            // Act
            var result = Assert.ThrowsAsync<UnhandledException>(async () => await _sut.MoveProductAsync(productMovimentationDto));

            // Assert
            Assert.That(result.Message, Is.EqualTo("Quantidade n�o pode ser atualizada."));
            //_productWarehouseQuantityServiceMock.Verify(v => v.UpdateProductWarehouseQuantityAsync(productMovimentationDto), Times.Once);
        }
    }
}