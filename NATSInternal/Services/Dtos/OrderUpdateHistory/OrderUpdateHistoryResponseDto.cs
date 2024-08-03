namespace NATSInternal.Services.Dtos;

public class OrderUpdateHistoryResponseDto
{
    private readonly OrderUpdateHistoryDataDto _oldData;
    private readonly OrderUpdateHistoryDataDto _newData;
    
    public DateTime UpdatedDateTime { get; set; }
    public UserBasicResponseDto UpdatedUser { get; set; }
    public string Reason { get; set; }
    
    public DateTime OldPaidDateTime => _oldData.PaidDateTime;
    public DateTime NewPaidDateTime => _newData.PaidDateTime;
    
    public string OldNote => _oldData.Note;
    public string NewNote => _newData.Note;
    
    public List<OrderItemUpdateHistoryDataDto> OldItems => _oldData.Items;
    public List<OrderItemUpdateHistoryDataDto> NewItems => _newData.Items;
    
    public OrderUpdateHistoryResponseDto(OrderUpdateHistory updateHistory)
    {
        _oldData = JsonSerializer
            .Deserialize<OrderUpdateHistoryDataDto>(updateHistory.OldData);
        _newData = JsonSerializer
            .Deserialize<OrderUpdateHistoryDataDto>(updateHistory.NewData);
        UpdatedDateTime = updateHistory.UpdatedDateTime;
        UpdatedUser = new UserBasicResponseDto(updateHistory.User);
        Reason = updateHistory.Reason;
    }
}