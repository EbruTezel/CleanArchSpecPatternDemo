using MediatR;

namespace ProductManagement.Application.Features.Product.Commands.Create;

public class CreateProductCommand : IRequest<Guid>
{
    public required string Name { get; set; }
    
    public decimal Price { get; set; }
    
    public int Stock { get; set; }
}
