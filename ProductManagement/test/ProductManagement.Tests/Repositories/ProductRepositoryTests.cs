using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Application.Features.Product.Spesifications;
using ProductManagement.Infrastructure.Persistence.DbContexts;
using ProductManagement.Infrastructure.Persistence.Repositories;
using ProductManagement.Tests.Helpers;

namespace ProductManagement.Tests.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        _context = TestDbContextFactory.CreateInMemoryDbContext();
        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ValidProduct_ShouldAddToDatabase()
    {
        // Arrange
        var product = new Domain.Product
        {
            Name = "Test Product",
            Price = 99.99m,
            Stock = 50
        };

        // Act
        var result = await _repository.AddAsync(product);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull("AddAsync should return the added product");
        result.Id.Should().NotBeEmpty("Product should have a valid ID");
        
        var savedProduct = await _context.Products.FindAsync(product.Id);
        savedProduct.Should().NotBeNull("Product should be saved to database");
        savedProduct!.Name.Should().Be("Test Product");
        savedProduct.Price.Should().Be(99.99m);
        savedProduct.Stock.Should().Be(50);
    }

    [Fact]
    public async Task AddRangeAsync_ValidProducts_ShouldAddAllToDatabase()
    {
        // Arrange
        var products = new[]
        {
            new Domain.Product { Name = "Product 1", Price = 100, Stock = 10 },
            new Domain.Product { Name = "Product 2", Price = 200, Stock = 20 },
            new Domain.Product { Name = "Product 3", Price = 300, Stock = 30 }
        };

        // Act
        await _repository.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Assert
        var savedProducts = await _context.Products.ToListAsync();
        savedProducts.Should().HaveCount(3, "All products should be saved");
        
        savedProducts.Should().Contain(p => p.Name == "Product 1" && p.Price == 100);
        savedProducts.Should().Contain(p => p.Name == "Product 2" && p.Price == 200);
        savedProducts.Should().Contain(p => p.Name == "Product 3" && p.Price == 300);
    }

    [Fact]
    public async Task Update_ExistingProduct_ShouldUpdateInDatabase()
    {
        // Arrange
        var product = new Domain.Product
        {
            Name = "Original Product",
            Price = 100,
            Stock = 10
        };

        await _repository.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        product.Name = "Updated Product";
        product.Price = 150;
        product.Stock = 15;
        
        var result = _repository.Update(product);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull("Update should return the updated product");
        
        var updatedProduct = await _context.Products.FindAsync(product.Id);
        updatedProduct.Should().NotBeNull("Updated product should exist in database");
        updatedProduct!.Name.Should().Be("Updated Product");
        updatedProduct.Price.Should().Be(150);
        updatedProduct.Stock.Should().Be(15);
    }

    [Fact]
    public async Task Delete_ExistingProduct_ShouldRemoveFromDatabase()
    {
        // Arrange
        var product = new Domain.Product
        {
            Name = "Product To Delete",
            Price = 100,
            Stock = 10
        };

        await _repository.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        _repository.Delete(product);
        await _context.SaveChangesAsync();

        // Assert
        var deletedProduct = await _context.Products.FindAsync(product.Id);
        deletedProduct.Should().BeNull("Product should be removed from database");
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithValidSpecification_ShouldReturnProduct()
    {
        // Arrange
        var targetId = Guid.NewGuid();
        var products = new[]
        {
            new Domain.Product { Name = "Product 1", Price = 100, Stock = 10 },
            new Domain.Product { Name = "Target Product", Price = 200, Stock = 20 },
            new Domain.Product { Name = "Product 3", Price = 300, Stock = 30 }
        };

        // Target product'ın ID'sini manuel set et
        typeof(Domain.Product).GetProperty("Id")!.SetValue(products[1], targetId);

        await _repository.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        var spec = new ProductByIdSpec(targetId);

        // Act
        var result = await _repository.FirstOrDefaultAsync(spec);

        // Assert
        result.Should().NotBeNull("Product should be found");
        result!.Id.Should().Be(targetId);
        result.Name.Should().Be("Target Product");
    }

    [Fact]
    public async Task ListAsync_WithSpecification_ShouldReturnMatchingProducts()
    {
        // Arrange
        var products = new[]
        {
            new Domain.Product { Name = "Expensive Product 1", Price = 1000, Stock = 5 },
            new Domain.Product { Name = "Cheap Product", Price = 50, Stock = 100 },
            new Domain.Product { Name = "Expensive Product 2", Price = 2000, Stock = 3 }
        };

        await _repository.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Create a simple specification for products with price > 500
        var expensiveProductsSpec = new TestExpensiveProductsSpec(500);

        // Act
        var result = await _repository.ListAsync(expensiveProductsSpec);

        // Assert
        result.Should().HaveCount(2, "Should return 2 expensive products");
        result.Should().OnlyContain(p => p.Price > 500, "All returned products should be expensive");
    }

    [Fact]
    public async Task CountAsync_WithSpecification_ShouldReturnCorrectCount()
    {
        // Arrange
        var products = new[]
        {
            new Domain.Product { Name = "Product 1", Price = 100, Stock = 10 },
            new Domain.Product { Name = "Product 2", Price = 200, Stock = 20 },
            new Domain.Product { Name = "Product 3", Price = 300, Stock = 30 }
        };

        await _repository.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        var expensiveProductsSpec = new TestExpensiveProductsSpec(150);

        // Act
        var count = await _repository.CountAsync(expensiveProductsSpec);

        // Assert
        count.Should().Be(2, "Should count 2 products with price > 150");
    }

    [Fact]
    public async Task AnyAsync_WithSpecification_ShouldReturnTrueIfExists()
    {
        // Arrange
        var products = new[]
        {
            new Domain.Product { Name = "Cheap Product", Price = 50, Stock = 10 }
        };

        await _repository.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        var expensiveProductsSpec = new TestExpensiveProductsSpec(1000);
        var cheapProductsSpec = new TestExpensiveProductsSpec(10);

        // Act
        var hasExpensiveProducts = await _repository.AnyAsync(expensiveProductsSpec);
        var hasCheapProducts = await _repository.AnyAsync(cheapProductsSpec);

        // Assert
        hasExpensiveProducts.Should().BeFalse("Should not find any expensive products");
        hasCheapProducts.Should().BeTrue("Should find at least one cheap product");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

// Test için basit bir Specification sınıfı
public class TestExpensiveProductsSpec : ProductManagement.Application.Persistence.Specifications.BaseSpecification<Domain.Product>
{
    public TestExpensiveProductsSpec(decimal minPrice) : base(p => p.Price > minPrice)
    {
    }
}