namespace NATSInternal.Services.Dtos;

public class CustomerDebtOperationResponseDto
{
    public DebtOperationType Operation { get; set; }
    public long Amount { get; set; }
    public DateTime OperatedDateTime { get; set; }
    public bool IsLocked { get; set; }
    public CustomerDebtOperationAuthorizationResponseDto Authorization { get; set; }
    
    public CustomerDebtOperationResponseDto(
            DebtIncurrence debt,
            IAuthorizationService authorizationService)
    {
        Operation = DebtOperationType.DebtIncurrence;
        Amount = debt.Amount;
        OperatedDateTime = debt.IncurredDateTime;
        IsLocked = debt.IsLocked;
        
        DebtIncurrenceAuthorizationResponseDto authorization;
        authorization = authorizationService.GetDebtAuthorization(debt);
        Authorization = new CustomerDebtOperationAuthorizationResponseDto(authorization);
    }
    
    public CustomerDebtOperationResponseDto(
            DebtPayment payment,
            IAuthorizationService authorizationService)
    {
        Operation = DebtOperationType.DebtPayment;
        Amount = payment.Amount;
        OperatedDateTime = payment.PaidDateTime;
        IsLocked = payment.IsLocked;
        
        DebtPaymentAuthorizationResponseDto authorization;
        authorization = authorizationService.GetDebtPaymentAuthorization(payment);
        Authorization = new CustomerDebtOperationAuthorizationResponseDto(authorization);
    }
}