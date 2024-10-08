namespace NATSInternal.Services.Dtos;

public class ConsultantUpsertRequestDto : IRequestDto<ConsultantUpsertRequestDto>
{
    public long Amount { get; set; }
    public string Note { get; set; }
    public DateTime? PaidDateTime { get; set; }
    public int CustomerId { get; set; }
    public CustomerUpsertRequestDto Customer { get; set; }
    public string UpdateReason { get; set; }

    public ConsultantUpsertRequestDto TransformValues()
    {
        Note = Note?.ToNullIfEmpty();
        return this;
    }
}