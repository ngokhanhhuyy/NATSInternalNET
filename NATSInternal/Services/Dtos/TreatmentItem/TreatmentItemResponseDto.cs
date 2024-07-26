namespace NATSInternal.Services.Dtos;

public class TreatmentItemResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public decimal VatFactor { get; set; }
    public int Quantity { get; set; }
    public ProductBasicResponseDto Product { get; set; }

    public TreatmentItemResponseDto() { }

    public TreatmentItemResponseDto(TreatmentItem treatmentItem)
    {
        Id = treatmentItem.Id;
        Amount = treatmentItem.Amount;
        VatFactor = treatmentItem.VatFactor;
        Quantity = treatmentItem.Quantity;
        Product = new ProductBasicResponseDto(treatmentItem.Product);
    }
}
