namespace NATSInternal.Services.Dtos;

public class SupplyItemUpdateHistoryDataDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public int SuppliedQuantity { get; set; }
    public string ProductName { get; set; }
    
    public SupplyItemUpdateHistoryDataDto(SupplyItem item)
    {
        Id = item.Id;
        Amount = item.Amount;
        SuppliedQuantity = item.SuppliedQuantity;
        ProductName = item.Product.Name;
    }
}