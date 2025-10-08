using FluentAssertions;
using Moq;
using ProductManagement.Application.Features.Product.Commands.Create;
using ProductManagement.Application.Persistence;
using ProductManagement.Application.Persistence.Repositories;
using ProductManagement.Domain;

namespace ProductManagement.Tests.Features.Product.Commands.Create;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProductRepository = new Mock<IProductRepository>();
        
        // UnitOfWork'ün ProductRepository'yi döndürmesini sağla
        _mockUnitOfWork.Setup(x => x.ProductRepository).Returns(_mockProductRepository.Object);
        
        _handler = new CreateProductCommandHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateProductAndReturnId()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Price = 100.50m,
            Stock = 5
        };

        var cancellationToken = CancellationToken.None;

        // Repository AddAsync method'unun çağrılacağını setup et
        _mockProductRepository
            .Setup(x => x.AddAsync(It.IsAny<Domain.Product>(), cancellationToken))
            .Returns((Domain.Product product, CancellationToken ct) => Task.FromResult(product));

        // UnitOfWork SaveChangesAsync method'unun çağrılacağını setup et
        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(cancellationToken))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeEmpty("Product should have a valid ID");

        // Repository AddAsync method'unun doğru parametrelerle çağrıldığını doğrula
        _mockProductRepository.Verify(
            x => x.AddAsync(
                It.Is<Domain.Product>(p => 
                    p.Name == command.Name && 
                    p.Price == command.Price && 
                    p.Stock == command.Stock),
                cancellationToken),
            Times.Once,
            "AddAsync should be called with correct product data");

        // SaveChangesAsync method'unun çağrıldığını doğrula
        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(cancellationToken),
            Times.Once,
            "SaveChangesAsync should be called once");
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldSetAuditProperties()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Audit Test Product",
            Price = 200.75m,
            Stock = 10
        };

        Domain.Product? capturedProduct = null;
        var cancellationToken = CancellationToken.None;

        // Repository AddAsync method'unu setup et ve product'ı yakala
        _mockProductRepository
            .Setup(x => x.AddAsync(It.IsAny<Domain.Product>(), cancellationToken))
            .Callback<Domain.Product, CancellationToken>((product, ct) => capturedProduct = product)
            .Returns((Domain.Product product, CancellationToken ct) => Task.FromResult(product));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(cancellationToken))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        capturedProduct.Should().NotBeNull("Product should be captured");
        capturedProduct!.Id.Should().NotBeEmpty("Product should have an ID");
        capturedProduct.CreateDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5), 
            "CreateDate should be set to current time");
        capturedProduct.IsDeleted.Should().BeFalse("New product should not be marked as deleted");
    }

    [Theory]
    [InlineData("", 100, 5)] // Empty name
    [InlineData("Valid Name", -50, 5)] // Negative price
    [InlineData("Valid Name", 100, -1)] // Negative stock
    public async Task Handle_InvalidCommand_ShouldStillProcessButWithInvalidData(
        string name, decimal price, int stock)
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = name,
            Price = price,
            Stock = stock
        };

        var cancellationToken = CancellationToken.None;

        _mockProductRepository
            .Setup(x => x.AddAsync(It.IsAny<Domain.Product>(), cancellationToken))
            .Returns((Domain.Product product, CancellationToken ct) => Task.FromResult(product));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(cancellationToken))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeEmpty("Handler should still return an ID even with invalid data");
        
        // Note: Bu test validation eksikliğini gösteriyor - gerçek uygulamada validation olmalı
        _mockProductRepository.Verify(
            x => x.AddAsync(It.IsAny<Domain.Product>(), cancellationToken),
            Times.Once,
            "AddAsync should be called even with invalid data");
    }
}