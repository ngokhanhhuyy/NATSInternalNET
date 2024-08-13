namespace NATSInternal.Services.Dtos;

public class DebtByCustomerBasicResponseDto
{
    public long Amount { get; set; }
    public long PaidAmount { get; set; }
    public long RemainingAmount { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    
    public DebtByCustomerBasicResponseDto(Customer customer)
    {
        Amount = customer.DebtAmount;
        PaidAmount = customer.DebtPaidAmount;
        RemainingAmount = customer.DebtRemainingAmount;
        Customer = new CustomerBasicResponseDto(customer);
    }
}
