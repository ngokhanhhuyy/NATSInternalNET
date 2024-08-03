namespace NATSInternal.Services.Dtos;

public class DebtListRequestDto : IRequestDto<DebtListRequestDto>
{
    public bool OrderByAscending { get; set; }
    public string OrderByField { get; set; } = nameof(FieldOptions.IncurredDateTime);
    public DateOnly? RangeFrom { get; set; }
    public DateOnly? RangeTo { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;
    
    public DebtListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty();
        return this;
    }
    
    public enum FieldOptions
    {
        IncurredDateTime,
        Amount
    }
}
