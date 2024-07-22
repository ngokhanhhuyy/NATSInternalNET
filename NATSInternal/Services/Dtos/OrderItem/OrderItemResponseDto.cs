namespace NATSInternal.Services.Dtos;

public class OrderItemResponseDto
{
    public int Id { get; set; }
    public long Amount { get; set; }
    public decimal VatFactor { get; set; }
    public int Quantity { get; set; }
    public ProductBasicResponseDto Product { get; set; }

    public OrderItemResponseDto(OrderItem item)
    {
        Id = item.Id;
        Amount = item.Amount;
        VatFactor = item.VatFactor;
        Quantity = item.Quantity;
        Product = new ProductBasicResponseDto(item.Product);
    }
}