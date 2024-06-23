namespace NATSInternal.Services.Dtos;

public class ExpenseListRequestDto : IRequestDto<ExpenseListRequestDto>
{
    public bool OrderByAscending { get; set; } = false;
    public string OrderByField { get; set; } = nameof(FieldOptions.PaidDateTime);
    public DateOnly? RangeFrom { get; set; }
    public DateOnly? RangeTo { get; set; }
    public ExpenseCategory? Category { get; set; }
    public int Page { get; set; }
    public int ResultsPerPage { get; set; }
    
    public ExpenseListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty();
        return this;
    }
    
    public enum FieldOptions
    {
        Amount,
        PaidDateTime
    }
}