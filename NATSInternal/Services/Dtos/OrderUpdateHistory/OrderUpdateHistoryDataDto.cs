namespace NATSInternal.Services.Dtos;

public class OrderUpdateHistoryDataDto
{
    public DateTime PaidDateTime { get; set; }
    public string Note { get; set; }
    public int CustomerId { get; set; }
    public List<OrderItemUpdateHistoryDataDto> Items { get; set; }

    public OrderUpdateHistoryDataDto() { }
    
    public OrderUpdateHistoryDataDto(Order order)
    {
        PaidDateTime = order.PaidDateTime;
        Note = order.Note;
        CustomerId = order.CustomerId;
        Items = order.Items
            .Select(i => new OrderItemUpdateHistoryDataDto(i))
            .ToList();
    }
}