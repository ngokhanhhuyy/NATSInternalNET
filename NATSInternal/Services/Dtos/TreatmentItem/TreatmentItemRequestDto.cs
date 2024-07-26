namespace NATSInternal.Services.Dtos;

public class TreatmentItemRequestDto : IRequestDto<TreatmentItemRequestDto>
{
    public int? Id { get; set; }
    public long Amount { get; set; }
    public decimal VatFactor { get; set; }
    public int Quantity { get; set; }
    public int ProductId { get; set; }
    public bool HasBeenChanged { get; set; }
    public bool HasBeenDeleted { get; set; }

    public TreatmentItemRequestDto TransformValues()
    {
        Id = Id == 0 ? null : Id;
        return this;
    }
}
