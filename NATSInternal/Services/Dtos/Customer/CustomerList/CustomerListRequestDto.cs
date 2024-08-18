namespace NATSInternal.Services.Dtos;

public class CustomerListRequestDto : IRequestDto<CustomerListRequestDto>
{
    public bool OrderByAscending { get; set; } = true;
    public string OrderByField { get; set; } = nameof(FieldToBeOrdered.LastName);
    public string SearchByContent { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;
    public bool HasRemainingDebtAmountOnly { get; set; }

    public CustomerListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty();
        SearchByContent = SearchByContent?.ToNullIfEmpty();
        return this;
    }

    public enum FieldToBeOrdered
    {
        LastName,
        FirstName,
        Birthday,
        CreatedDateTime,
        DebtRemainingAmount
    }
}