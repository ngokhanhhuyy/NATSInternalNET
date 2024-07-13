namespace NATSInternal.Services.Dtos;

public class DebtPaymentRequestDto : IRequestDto<DebtPaymentRequestDto>
{
    public int? Id { get; set; }
    public long Amount { get; set; }
    public DateTime? PaidDateTime { get; set; }
    public string Note { get; set; }
    public bool HasBeenChanged { get; set; }
    public bool HasBeenDeleted { get; set; }
        
    public DebtPaymentRequestDto TransformValues()
    {
            Note = Note?.ToNullIfEmpty();
            return this;
        }
}