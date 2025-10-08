using ProductManagement.Application.Persistence.Repositories;
using ProductManagement.Application.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain;
using ProductManagement.Infrastructure.Persistence.Specifications;
using ProductManagement.Infrastructure.Persistence.DbContexts;


namespace ProductManagement.Infrastructure.Persistence.Repositories;

public class ProductRepository(ApplicationDbContext dbContext) : GenericRepository<Product, ApplicationDbContext>(dbContext), IProductRepository;