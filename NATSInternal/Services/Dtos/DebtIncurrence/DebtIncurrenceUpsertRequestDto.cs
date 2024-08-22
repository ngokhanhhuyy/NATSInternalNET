namespace NATSInternal.Services.Dtos;

public class DebtIncurrenceUpsertRequestDto : IRequestDto<DebtIncurrenceUpsertRequestDto>
{
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime? IncurredDateTime { get; set; }
    public string UpdatingReason { get; set; }
    
    public DebtIncurrenceUpsertRequestDto TransformValues()
    {
        Note = Note?.ToNullIfEmpty();
        return this;
    }
}