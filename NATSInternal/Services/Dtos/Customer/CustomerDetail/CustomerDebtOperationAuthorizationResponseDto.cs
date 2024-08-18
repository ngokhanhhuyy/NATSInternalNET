namespace NATSInternal.Services.Dtos;

public class CustomerDebtOperationAuthorizationResponseDto
{
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    
    public CustomerDebtOperationAuthorizationResponseDto(
            DebtAuthorizationResponseDto authorization)
    {
        CanEdit = authorization.CanEdit;
        CanDelete = authorization.CanDelete;
    }
    
    public CustomerDebtOperationAuthorizationResponseDto(
            DebtPaymentAuthorizationResponseDto authorization)
    {
        CanEdit = authorization.CanEdit;
        CanDelete = authorization.CanDelete;
    }
}