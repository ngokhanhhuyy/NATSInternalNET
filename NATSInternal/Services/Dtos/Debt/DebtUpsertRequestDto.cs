namespace NATSInternal.Services.Dtos;

public class DebtUpsertRequestDto : IRequestDto<DebtUpsertRequestDto>
{
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime? IncurredDateTime { get; set; }
    public int CustomerId { get; set; }
    public string UpdatingReason { get; set; }
    
    public DebtUpsertRequestDto TransformValues()
    {
        Note = Note?.ToNullIfEmpty();
        return this;
    }
}