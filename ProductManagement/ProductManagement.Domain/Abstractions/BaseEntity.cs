namespace ProductManagement.Domain.Abstractions;

public abstract class BaseEntity(Guid id)
{
    public Guid Id { get; protected set; } = id;
    

    public BaseEntity() : this(Guid.NewGuid())
    {
        
    }
}