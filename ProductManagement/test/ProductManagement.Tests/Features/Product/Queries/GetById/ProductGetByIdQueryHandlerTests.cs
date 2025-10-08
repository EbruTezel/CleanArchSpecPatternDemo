using AutoMapper;
using FluentAssertions;
using Moq;
using ProductManagement.Application.Features.Product.Dtos;
using ProductManagement.Application.Features.Product.Queries.GetById;
using ProductManagement.Application.Features.Product.Spesifications;
using ProductManagement.Application.Persistence;
using ProductManagement.Application.Persistence.Repositories;
using ProductManagement.Application.Persistence.Specifications;

namespace ProductManagement.Tests.Features.Product.Queries.GetById;

public class ProductGetByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductGetByIdQueryHandler _handler;

    public ProductGetByIdQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockMapper = new Mock<IMapper>();

        _mockUnitOfWork.Setup(x => x.ProductRepository).Returns(_mockProductRepository.Object);

        _handler = new ProductGetByIdQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ExistingProductId_ShouldReturnProductDto()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new ProductGetByIdQuery { Id = productId };

        var product = new Domain.Product
        {
            Name = "Test Product",
            Price = 99.99m,
            Stock = 10
        };
        
        // Product ID'sini set et
        typeof(Domain.Product).GetProperty("Id")!.SetValue(product, productId);

        var expectedDto = new ProductDto
        {
            Id = productId,
            Name = "Test Product",
            Price = 99.99m,
            Stock = 10,
            CreateDate = DateTime.UtcNow,
            IsDeleted = false
        };

        // Repository setup
        _mockProductRepository
            .Setup(x => x.FirstOrDefaultAsync(
                It.Is<ProductByIdSpec>(spec => 
                    spec.Criteria.Compile().Invoke(product) == true), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Mapper setup
        _mockMapper
            .Setup(x => x.Map<ProductDto>(product))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull("Result should not be null for existing product");
        result.Id.Should().Be(productId);
        result.Name.Should().Be("Test Product");
        result.Price.Should().Be(99.99m);
        result.Stock.Should().Be(10);

        // Verify repository was called with correct specification
        _mockProductRepository.Verify(
            x => x.FirstOrDefaultAsync(
                It.IsAny<ProductByIdSpec>(), 
                It.IsAny<CancellationToken>()),
            Times.Once,
            "Repository should be called once with ProductByIdSpec");

        // Verify mapper was called
        _mockMapper.Verify(
            x => x.Map<ProductDto>(product),
            Times.Once,
            "Mapper should be called once to map Product to ProductDto");
    }

    [Fact]
    public async Task Handle_NonExistingProductId_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        var query = new ProductGetByIdQuery { Id = nonExistingId };

        // Repository setup - return null for non-existing product
        _mockProductRepository
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<ProductByIdSpec>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Product?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull("Result should be null for non-existing product");

        // Verify repository was called
        _mockProductRepository.Verify(
            x => x.FirstOrDefaultAsync(
                It.IsAny<ProductByIdSpec>(), 
                It.IsAny<CancellationToken>()),
            Times.Once,
            "Repository should be called once");

        // Verify mapper was NOT called since product is null
        _mockMapper.Verify(
            x => x.Map<ProductDto>(It.IsAny<Domain.Product>()),
            Times.Never,
            "Mapper should not be called when product is null");
    }

    [Fact]
    public async Task Handle_EmptyGuidId_ShouldReturnNull()
    {
        // Arrange
        var query = new ProductGetByIdQuery { Id = Guid.Empty };

        _mockProductRepository
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<ProductByIdSpec>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Product?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull("Result should be null for empty GUID");

        _mockProductRepository.Verify(
            x => x.FirstOrDefaultAsync(
                It.IsAny<ProductByIdSpec>(), 
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidQuery_ShouldUseCorrectSpecification()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new ProductGetByIdQuery { Id = productId };

        ProductByIdSpec? capturedSpec = null;

        _mockProductRepository
            .Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<ISpecification<Domain.Product>>(), 
                It.IsAny<CancellationToken>()))
            .Callback<ISpecification<Domain.Product>, CancellationToken>((spec, ct) => capturedSpec = spec as ProductByIdSpec)
            .ReturnsAsync((Domain.Product?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        capturedSpec.Should().NotBeNull("Specification should be captured");
        
        // Test specification criteria with a mock product
        var testProduct = new Domain.Product { Name = "Test", Price = 100, Stock = 10 };
        typeof(Domain.Product).GetProperty("Id")!.SetValue(testProduct, productId);
        
        var matchesSpec = capturedSpec!.Criteria.Compile().Invoke(testProduct);
        matchesSpec.Should().BeTrue("Specification should match product with correct ID");

        // Test with different ID
        var differentProduct = new Domain.Product { Name = "Test", Price = 100, Stock = 10 };
        typeof(Domain.Product).GetProperty("Id")!.SetValue(differentProduct, Guid.NewGuid());
        
        var doesNotMatchSpec = capturedSpec.Criteria.Compile().Invoke(differentProduct);
        doesNotMatchSpec.Should().BeFalse("Specification should not match product with different ID");
    }
}