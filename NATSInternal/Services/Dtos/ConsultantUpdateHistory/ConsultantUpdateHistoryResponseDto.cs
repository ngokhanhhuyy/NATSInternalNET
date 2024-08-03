namespace NATSInternal.Services.Dtos;

public class ConsultantUpdateHistoryResponseDto
{
    private readonly ConsultantUpdateHistoryDataDto _oldData;
    private readonly ConsultantUpdateHistoryDataDto _newData;
    
    public DateTime UpdatedDateTime { get; private set; }
    public UserBasicResponseDto UpdatedUser { get; private set; }
    public string Reason { get; private set; }
    
    public DateTime OldPaidDateTime => _oldData.PaidDateTime;
    public DateTime NewPaidDateTime => _newData.PaidDateTime;
    
    public long OldAmount => _oldData.Amount;
    public long NewAmount => _newData.Amount;
    
    public string OldNote => _oldData.Note;
    public string NewNote => _newData.Note;
    
    public ConsultantUpdateHistoryResponseDto(ConsultantUpdateHistory updateHistory)
    {
        _oldData = JsonSerializer
            .Deserialize<ConsultantUpdateHistoryDataDto>(updateHistory.OldData);
        _newData = JsonSerializer
            .Deserialize<ConsultantUpdateHistoryDataDto>(updateHistory.NewData);
        UpdatedDateTime = updateHistory.UpdatedDateTime;
        UpdatedUser = new UserBasicResponseDto(updateHistory.User);
        Reason = updateHistory.Reason;
    }
}