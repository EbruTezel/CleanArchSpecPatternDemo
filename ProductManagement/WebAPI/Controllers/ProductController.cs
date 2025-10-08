using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.Features.Product.Queries.GetById;
using ProductManagement.Application.Features.Product.Commands.Create;
using ProductManagement.Application.Features.Product.Dtos;
using ProductManagement.Application.Common.Responses;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ProductController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new ProductGetByIdQuery { Id = id });
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var productId = await _mediator.Send(command);
        return Ok(ApiResponse<Guid>.Ok(productId, "Product created successfully"));
    }
}