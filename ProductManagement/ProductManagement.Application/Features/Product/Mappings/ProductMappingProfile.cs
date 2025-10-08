using AutoMapper;
using ProductManagement.Application.Features.Product.Commands.Create;
using ProductManagement.Application.Features.Product.Dtos;

namespace ProductManagement.Application.Features.Product.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<ProductDto, Domain.Product>();
        CreateMap<Domain.Product, ProductDto>();

        CreateMap<CreateProductCommand, Domain.Product>();
        CreateMap<Domain.Product, CreateProductCommand>();
    }
}