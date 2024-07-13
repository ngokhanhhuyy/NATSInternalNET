namespace NATSInternal.Services.Dtos;

public class DebtListRequestDto : IRequestDto<DebtListRequestDto>
{
    public bool OrderByAscending { get; set; }
    public string OrderByField { get; set; } = nameof(FieldOptions.CreatedDateTime);
    public DateOnly? RangeFrom { get; set; }
    public DateOnly? RangeTo { get; set; }
    public int Page { get; set; }
    public int ResultsPerPage { get; set; }
    
    public DebtListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty();
        return this;
    }
    
    public enum FieldOptions
    {
        CreatedDateTime,
        Amount
    }
}
