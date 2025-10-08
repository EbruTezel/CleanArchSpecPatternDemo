using MediatR;
using ProductManagement.Application.Features.Product.Dtos;

namespace ProductManagement.Application.Features.Product.Queries.GetById;

public class ProductGetByIdQuery : IRequest<ProductDto?>
{
    public Guid Id { get; set; }
}