namespace NATSInternal.Services.Dtos;

public class DebtUpdateHistoryResponseDto
{
    private readonly DebtUpdateHistoryDataDto _oldData;
    private readonly DebtUpdateHistoryDataDto _newData;
    
    public DateTime UpdatedDateTime { get; private set; }
    public UserBasicResponseDto UpdatedUser { get; private set; }
    public string Reason { get; private set; }
    
    public DateTime OldIncurredDateTime => _oldData.IncurredDateTime;
    public DateTime NewInCurredDateTime => _newData.IncurredDateTime;
    
    public long OldAmount => _oldData.Amount;
    public long NewAmount => _newData.Amount;
    
    public string OldNote => _oldData.Note;
    public string NewNote => _newData.Note;
    
    public DebtUpdateHistoryResponseDto(DebtUpdateHistory updateHistory)
    {
        _oldData = JsonSerializer
            .Deserialize<DebtUpdateHistoryDataDto>(updateHistory.OldData);
        _newData = JsonSerializer
            .Deserialize<DebtUpdateHistoryDataDto>(updateHistory.NewData);
        UpdatedDateTime = updateHistory.UpdatedDateTime;
        UpdatedUser = new UserBasicResponseDto(updateHistory.User);
        Reason = updateHistory.Reason;
    }
}