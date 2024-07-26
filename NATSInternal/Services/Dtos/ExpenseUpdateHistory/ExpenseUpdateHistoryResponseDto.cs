namespace NATSInternal.Services.Dtos;

public class ExpenseUpdateHistoryResponseDto
{
    private ExpenseUpdateHistoryDataDto _oldData;
    private ExpenseUpdateHistoryDataDto _newData;
    
    public long OldAmount => _oldData.Amount;
    public long NewAmount => _newData.Amount;
    
    public DateTime OldPaidDateTime => _oldData.PaidDateTime;
}