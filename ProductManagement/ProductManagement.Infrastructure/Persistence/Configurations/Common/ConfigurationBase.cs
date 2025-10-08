using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManagement.Domain.Abstractions;

namespace ProductManagement.Infrastructure.Persistence.Configurations.Common;

public class ConfigurationBase<T> : IEntityTypeConfiguration<T> 
    where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
    }
}