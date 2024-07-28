namespace NATSInternal.Services.Dtos;

public class TreatmentItemUpdateHistoryDataDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public decimal VatFactor { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; }

    public TreatmentItemUpdateHistoryDataDto(TreatmentItem item)
    {
        Id = item.Id;
        Amount = item.Amount;
        VatFactor = item.VatFactor;
        Quantity = item.Quantity;
        ProductName = item.Product.Name;
    }
}