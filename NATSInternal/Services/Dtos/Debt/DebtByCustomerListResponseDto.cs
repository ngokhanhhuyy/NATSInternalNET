namespace NATSInternal.Services.Dtos;

public class DebtByCustomerListResponseDto
{
    public int PageCount { get; set; }
    public List<DebtByCustomerBasicResponseDto> Items { get; set; }
    public DebtListAuthorizationResponseDto DebtAuthorization { get; set; }
    public DebtPaymentListAuthorizationResponseDto DebtPaymentAuthorization { get; set; }
}
