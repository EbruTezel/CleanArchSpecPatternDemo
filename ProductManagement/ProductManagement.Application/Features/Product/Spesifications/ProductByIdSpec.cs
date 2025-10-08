using ProductManagement.Application.Persistence.Specifications;

namespace ProductManagement.Application.Features.Product.Spesifications;

public class ProductByIdSpec : BaseSpecification<Domain.Product>
{
    public ProductByIdSpec(Guid id, bool asNoTracking = true, bool asSplitQuery = true)
    {
        Criteria = c => c.Id == id && !c.IsDeleted;
        
        AsNoTracking = asNoTracking;
        AsSplitQuery = asSplitQuery;
    }
}