namespace NATSInternal.Services.Dtos;

public class OrderItemUpdateHistoryDataDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public decimal VatFactor { get; set; }
    public int Quantity { get; set; }
    public int ProductId { get; set; }
    
    public OrderItemUpdateHistoryDataDto(OrderItem item)
    {
        Id = item.Id;
        Amount = item.Amount;
        VatFactor = item.VatFactor;
        Quantity = item.Quantity;
        ProductId = item.ProductId;
    }
}