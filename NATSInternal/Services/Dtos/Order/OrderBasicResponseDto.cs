namespace NATSInternal.Services.Dtos;

public class OrderBasicResponseDto
{
    public int Id { get; set; }
    public DateTime PaidDateTime { get; set; }
    public long Amount { get; set; }
    public bool IsLocked { get; set; }  
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
        PaidDateTime = order.PaidDateTime;
        Amount = order.ItemAmount;
        IsLocked = order.IsLocked;
        Customer = new CustomerBasicResponseDto(order.Customer);
    }
}