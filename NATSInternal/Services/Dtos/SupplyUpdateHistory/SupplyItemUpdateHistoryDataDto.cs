namespace NATSInternal.Services.Dtos;

public class SupplyItemUpdateHistoryDataDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public int SuppliedQuantity { get; set; }
    public int ProductId { get; set; }
    
    public SupplyItemUpdateHistoryDataDto(SupplyItem item)
    {
        Id = item.Id;
        Amount = item.Amount;
        SuppliedQuantity = item.SuppliedQuantity;
        ProductId = item.ProductId;
    }
}