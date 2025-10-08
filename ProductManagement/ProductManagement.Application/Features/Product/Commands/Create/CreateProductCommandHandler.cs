using MediatR;
using ProductManagement.Application.Persistence;

namespace ProductManagement.Application.Features.Product.Commands.Create;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Domain.Product
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock,
            CreateDate = DateTime.UtcNow
        };
        
        await _unitOfWork.ProductRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}
