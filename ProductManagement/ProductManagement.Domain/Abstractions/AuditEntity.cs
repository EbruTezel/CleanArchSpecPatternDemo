namespace ProductManagement.Domain.Abstractions;

public abstract class AuditEntity : BaseEntity
{
    public bool IsDeleted { get; protected set; }
    
    public DateTime CreateDate { get; set; }
    
    public string? CreatedBy { get; set; } 
    
    public DateTime? UpdateDate { get; set; }
    
    public string? UpdateBy { get; set; } 
    
    protected AuditEntity() : base(){}
    
    protected AuditEntity(Guid id) : base(id){}
}