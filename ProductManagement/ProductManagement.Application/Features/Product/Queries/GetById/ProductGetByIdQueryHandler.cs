using AutoMapper;
using MediatR;
using ProductManagement.Application.Features.Product.Dtos;
using ProductManagement.Application.Features.Product.Spesifications;
using ProductManagement.Application.Persistence;

namespace ProductManagement.Application.Features.Product.Queries.GetById;

public class ProductGetByIdQueryHandler : IRequestHandler<ProductGetByIdQuery, ProductDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper mapper;

    public ProductGetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<ProductDto?> Handle(ProductGetByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new ProductByIdSpec(request.Id);
        var product = await _unitOfWork.ProductRepository.FirstOrDefaultAsync(spec, cancellationToken);

        return product is null ? null : mapper.Map<ProductDto>(product);
    }

}