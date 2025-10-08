using ProductManagement.Domain.Abstractions;

namespace ProductManagement.Domain;

public class Product : AuditEntity
{
    public string Name { get; set; } = null!;
    
    public decimal Price { get; set; }
    
    public int Stock { get; set; }
}