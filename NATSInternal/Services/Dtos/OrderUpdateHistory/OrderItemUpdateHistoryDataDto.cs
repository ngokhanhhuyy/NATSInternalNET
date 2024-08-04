namespace NATSInternal.Services.Dtos;

public class OrderItemUpdateHistoryDataDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public decimal VatFactor { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; }

    public OrderItemUpdateHistoryDataDto() { }
    
    public OrderItemUpdateHistoryDataDto(OrderItem item)
    {
        Id = item.Id;
        Amount = item.Amount;
        VatFactor = item.VatFactor;
        Quantity = item.Quantity;
        ProductName = item.Product.Name;
    }
}