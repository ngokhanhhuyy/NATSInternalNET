namespace NATSInternal.Services.Dtos;

public class SupplyUpdateHistoryResponseDto
{
    private readonly SupplyUpdateHistoryDataDto _oldData;
    private readonly SupplyUpdateHistoryDataDto _newData;
    
    public DateTime UpdatedDateTime { get; private set; }
    public UserBasicResponseDto UpdatedUser { get; private set; }
    public string Reason { get; private set; }
    
    public DateTime OldPaidDateTime => _oldData.PaidDateTime;
    public DateTime NewPaidDateTime => _newData.PaidDateTime;
    
    public long OldShipmentFee => _oldData.ShipmentFee;
    public long NewShipementFee => _newData.ShipmentFee;
    
    public string OldNote => _oldData.Note;
    public string NewNote => _newData.Note;
    
    public List<SupplyItemUpdateHistoryDataDto> OldItems => _oldData.Items;
    public List<SupplyItemUpdateHistoryDataDto> NewItems => _newData.Items;
    
    public SupplyUpdateHistoryResponseDto(SupplyUpdateHistory updateHistory)
    {
        _oldData = JsonSerializer.Deserialize<SupplyUpdateHistoryDataDto>(updateHistory.OldData);
        _newData = JsonSerializer.Deserialize<SupplyUpdateHistoryDataDto>(updateHistory.NewData);
        UpdatedDateTime = updateHistory.UpdatedDateTime;
        UpdatedUser = new UserBasicResponseDto(updateHistory.User);
        Reason = updateHistory.Reason;
    }
}