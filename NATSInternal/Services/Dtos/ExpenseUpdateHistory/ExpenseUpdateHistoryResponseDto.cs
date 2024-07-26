namespace NATSInternal.Services.Dtos;

public class ExpenseUpdateHistoryResponseDto
{
    private ExpenseUpdateHistoryDataDto _oldData;
    private ExpenseUpdateHistoryDataDto _newData;

    public DateTime UpdatedDateTime { get; set; }
    public UserBasicResponseDto UpdatedUser { get; set; }
    
    public long OldAmount => _oldData.Amount;
    public long NewAmount => _newData.Amount;
    
    public DateTime OldPaidDateTime => _oldData.PaidDateTime;
    public DateTime NewPaidDateTime => _newData.PaidDateTime;

    public ExpenseCategory OldCategory => _oldData.Category;
    public ExpenseCategory NewCategory => _newData.Category;

    public string OldNote => _oldData.Note;
    public string NewNote => _newData.Note;

    public string OldPayeeName => _oldData.PayeeName;
    public string NewPayeeName => _newData.PayeeName;

    public ExpenseUpdateHistoryResponseDto(ExpenseUpdateHistory updateHistory)
    {
        _oldData = JsonSerializer.Deserialize<ExpenseUpdateHistoryDataDto>(updateHistory.OldData);
        _newData = JsonSerializer.Deserialize<ExpenseUpdateHistoryDataDto>(updateHistory.NewData);
        UpdatedDateTime = updateHistory.UpdatedDateTime;
        UpdatedUser = new UserBasicResponseDto(updateHistory.User);
    }
}