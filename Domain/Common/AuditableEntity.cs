namespace Domain.Common;

public class AuditableEntity : Entity
{
    public required DateTime Created { get; set; } 
    public string? CreatedBy { get; set; }
    public DateTime? Modified { get; set; } 
    public string? ModifiedBy { get; set; }
}