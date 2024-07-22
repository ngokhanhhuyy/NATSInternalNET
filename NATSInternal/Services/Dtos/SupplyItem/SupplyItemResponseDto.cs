namespace NATSInternal.Services.Dtos;

public class SupplyItemResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public int SuppliedQuantity { get; set; }
    public ProductBasicResponseDto Product { get; set; }

    public SupplyItemResponseDto(SupplyItem item)
    {
        Id = item.Id;
        Amount = item.Amount;
        SuppliedQuantity = item.SuppliedQuantity;
        Product = new ProductBasicResponseDto(item.Product);
    }
}
