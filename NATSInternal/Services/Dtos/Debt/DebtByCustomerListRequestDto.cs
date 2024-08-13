namespace NATSInternal.Services.Dtos;

public class DebtByCustomerListRequestDto : IRequestDto<DebtByCustomerListRequestDto>
{
    public string OrderByField { get; set; } = nameof(FieldOptions.RemainingAmount);
    public bool OrderByAscending { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;

    public DebtByCustomerListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty();
        return this;
    }
    
    public enum FieldOptions
    {
        Amount,
        PaidAmount,
        RemainingAmount
    }
}
