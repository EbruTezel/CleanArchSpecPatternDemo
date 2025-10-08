using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Application.Features.Product.Spesifications;
using ProductManagement.Infrastructure.Persistence.DbContexts;
using ProductManagement.Infrastructure.Persistence.Specifications;
using ProductManagement.Tests.Helpers;

namespace ProductManagement.Tests.Specifications;

public class ProductByIdSpecTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public ProductByIdSpecTests()
    {
        _context = TestDbContextFactory.CreateInMemoryDbContext();
    }

    [Fact]
    public async Task ProductByIdSpec_WithValidId_ShouldReturnCorrectProduct()
    {
        // Arrange
        var targetId = Guid.NewGuid();

        var products = new[]
        {
            new Domain.Product { Name = "Product 1", Price = 100, Stock = 10 },
            new Domain.Product { Name = "Target Product", Price = 200, Stock = 20 },
            new Domain.Product { Name = "Product 3", Price = 300, Stock = 30 }
        };

        // Target product'ın ID'sini manuel olarak set et
        var idProperty = typeof(Domain.Product).GetProperty("Id")!;
        idProperty.SetValue(products[1], targetId);

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        var spec = new ProductByIdSpec(targetId);

        // Act
        var query = SpecificationEvaluator.GetQuery(_context.Products.AsQueryable(), spec);
        var result = await query.FirstOrDefaultAsync();

        // Assert
        result.Should().NotBeNull("Product with the specified ID should be found");
        result!.Id.Should().Be(targetId, "The correct product should be returned");
        result.Name.Should().Be("Target Product", "Product properties should match");
        result.Price.Should().Be(200, "Product price should match");
        result.Stock.Should().Be(20, "Product stock should match");
    }

    [Fact]
    public async Task ProductByIdSpec_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var products = new[]
        {
            new Domain.Product { Name = "Product 1", Price = 100, Stock = 10 },
            new Domain.Product { Name = "Product 2", Price = 200, Stock = 20 }
        };

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        var nonExistentId = Guid.NewGuid();
        var spec = new ProductByIdSpec(nonExistentId);

        // Act
        var query = SpecificationEvaluator.GetQuery(_context.Products.AsQueryable(), spec);
        var result = await query.FirstOrDefaultAsync();

        // Assert
        result.Should().BeNull("No product should be found with non-existent ID");
    }

    [Fact]
    public async Task ProductByIdSpec_WithEmptyGuid_ShouldReturnNull()
    {
        // Arrange
        var products = new[]
        {
            new Domain.Product { Name = "Product 1", Price = 100, Stock = 10 }
        };

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        var spec = new ProductByIdSpec(Guid.Empty);

        // Act
        var query = SpecificationEvaluator.GetQuery(_context.Products.AsQueryable(), spec);
        var result = await query.FirstOrDefaultAsync();

        // Assert
        result.Should().BeNull("No product should be found with empty GUID");
    }

    [Fact]
    public async Task ProductByIdSpec_ShouldNotReturnDeletedProducts()
    {
        // Arrange
        var targetId = Guid.NewGuid();
        var product = new Domain.Product { Name = "Deleted Product", Price = 100, Stock = 10 };
        
        // ID'yi ve IsDeleted'ı manuel olarak set et
        var idProperty = typeof(Domain.Product).GetProperty("Id")!;
        var isDeletedProperty = typeof(Domain.Product).GetProperty("IsDeleted")!;
        
        idProperty.SetValue(product, targetId);
        isDeletedProperty.SetValue(product, true);

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var spec = new ProductByIdSpec(targetId);

        // Act
        var query = SpecificationEvaluator.GetQuery(_context.Products.AsQueryable(), spec);
        var result = await query.FirstOrDefaultAsync();

        // Assert
        result.Should().BeNull("Deleted products should not be returned by specification");
    }

    [Fact]
    public void ProductByIdSpec_Properties_ShouldBeSetCorrectly()
    {
        // Arrange
        var targetId = Guid.NewGuid();
        var spec = new ProductByIdSpec(targetId);

        // Assert
        spec.Criteria.Should().NotBeNull("Criteria should be set");
        spec.IsPagingEnabled.Should().BeFalse("Paging should be disabled for single item queries");
        spec.Includes.Should().BeEmpty("No includes should be specified for simple ID lookup");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}