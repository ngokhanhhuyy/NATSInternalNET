namespace NATSInternal.Services.Dtos;

public class DebtPaymentUpdateHistoryResponseDto
{
    private readonly DebtPaymentUpdateHistoryDataDto _oldData;
    private readonly DebtPaymentUpdateHistoryDataDto _newData;
    
    public DateTime UpdatedDateTime { get; private set; }
    public UserBasicResponseDto UpdatedUser { get; private set; }
    
    public long OldAmount => _oldData.Amount;
    public long NewAmount => _newData.Amount;
    
    public string OldNote => _oldData.Note;
    public string NewNote => _oldData.Note;
    
    public DateTime OldPaidDateTime => _oldData.PaidDateTime;
    public DateTime NewPaidDateTime => _newData.PaidDateTime;
    
    public DebtPaymentUpdateHistoryResponseDto(DebtPaymentUpdateHistory updateHistory)
    {
        _oldData = JsonSerializer
            .Deserialize<DebtPaymentUpdateHistoryDataDto>(updateHistory.OldData);
        _newData = JsonSerializer
            .Deserialize<DebtPaymentUpdateHistoryDataDto>(updateHistory.NewData);
        UpdatedDateTime = updateHistory.UpdatedDateTime;
        UpdatedUser = new UserBasicResponseDto(updateHistory.User);
    }
}