namespace NATSInternal.Services.Dtos;

public class OrderDetailResponseDto
{
    public int Id { get; set; }
    public DateTime PaidDateTime { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public long BeforeVatAmount { get; set; }
    public long VatAmount { get; set; }
    public long AfterVatAmount { get; set; }
    public string Note { get; set; }
    public bool IsLocked { get; set; }
    public List<OrderItemResponseDto> Items { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public UserBasicResponseDto User { get; set; }
    public List<OrderPhotoResponseDto> Photos { get; set; }
    public OrderAuthorizationResponseDto Authorization { get; set; }
    public List<OrderUpdateHistoryResponseDto> UpdateHistories { get; set; }

    public OrderDetailResponseDto(
            Order order,
            OrderAuthorizationResponseDto authorization,
            bool mapUpdateHistories = false)
    {
        Id = order.Id;
        PaidDateTime = order.PaidDateTime;
        CreatedDateTime = order.CreatedDateTime;
        BeforeVatAmount = order.BeforeVatAmount;
        AfterVatAmount = order.AfterVatAmount;
        Note = order.Note;
        IsLocked = order.IsLocked;
        Items = order.Items?.Select(i => new OrderItemResponseDto(i)).ToList();
        Customer = new CustomerBasicResponseDto(order.Customer);
        User = new UserBasicResponseDto(order.CreatedUser);
        Photos = order.Photos?.Select(p => new OrderPhotoResponseDto(p)).ToList();
        Authorization = authorization;

        if (mapUpdateHistories)
        {
            UpdateHistories = order.UpdateHistories
                .Select(uh => new OrderUpdateHistoryResponseDto(uh))
                .ToList();
        }
    }
}