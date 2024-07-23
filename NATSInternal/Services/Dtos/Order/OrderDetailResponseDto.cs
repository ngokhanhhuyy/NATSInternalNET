namespace NATSInternal.Services.Dtos;

public class OrderDetailResponseDto
{
    public int Id { get; set; }
    public DateTime OrderedDateTime { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public long AmountBeforeVAT { get; set; }
    public long AmountAfterVAT { get; set; }
    public string Note { get; set; }
    public bool IsLocked { get; set; }
    public List<OrderItemResponseDto> Items { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public UserBasicResponseDto User { get; set; }
    public List<OrderPhotoResponseDto> Photos { get; set; }
    public OrderAuthorizationResponseDto Authorization { get; set; }

    public OrderDetailResponseDto(
            Order order,
            OrderAuthorizationResponseDto authorization)
    {
        Id = order.Id;
        OrderedDateTime = order.PaidDateTime;
        CreatedDateTime = order.CreatedDateTime;
        AmountBeforeVAT = order.ItemAmount;
        AmountAfterVAT = order.ItemAmount + order.VatAmount;
        Note = order.Note;
        IsLocked = order.IsLocked;
        Items = order.Items?.Select(i => new OrderItemResponseDto(i)).ToList();
        Customer = new CustomerBasicResponseDto(order.Customer);
        User = new UserBasicResponseDto(order.CreatedUser);
        Photos = order.Photos?.Select(p => new OrderPhotoResponseDto(p)).ToList();
        Authorization = authorization;
    }
}