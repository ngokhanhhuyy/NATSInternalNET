namespace NATSInternal.Services.Dtos;

public class SupplyItemResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public decimal VatFactor { get; set; }
    public int SuppliedQuantity { get; set; }
    public ProductBasicResponseDto Product { get; set; }
}
