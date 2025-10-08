using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagement.Domain;

namespace ProductManagement.Infrastructure.Persistence.Configurations.Common;

public class ProductConfiguration : ConfigurationBase<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);
    }
}