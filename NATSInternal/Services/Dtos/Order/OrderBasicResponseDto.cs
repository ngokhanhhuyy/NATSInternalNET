namespace NATSInternal.Services.Dtos;

public class OrderBasicResponseDto
{
    public int Id { get; set; }
    public DateTime OrderedDateTime { get; set; }
    public long Amount { get; set; }
    public bool IsClosed { get; set; }  
    public CustomerBasicResponseDto Customer { get; set; }
    public OrderAuthorizationResponseDto Authorization { get; set; }

    public OrderBasicResponseDto(Order order)
    {
        MapFromEntity(order);
    }

    public OrderBasicResponseDto(
            Order order,
            OrderAuthorizationResponseDto authorization)
    {
        MapFromEntity(order);
        Authorization = authorization;
    }

    private void MapFromEntity(Order order)
    {
        Id = order.Id;
        OrderedDateTime = order.OrderedDateTime;
        Amount = order.ItemAmount;
        IsClosed = order.IsClosed;
        Customer = new CustomerBasicResponseDto(order.Customer);
    }
}