namespace NATSInternal.Services.Dtos;

public class ExpenseListRequestDto : IRequestDto<ExpenseListRequestDto>, ILockableEntityListRequestDto
{
    public bool OrderByAscending { get; set; } = false;
    public string OrderByField { get; set; } = nameof(FieldOptions.PaidDateTime);
    public int? Month { get; set; }
    public int? Year { get; set; }
    public ExpenseCategory? Category { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;
    
    public ExpenseListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty();
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        Month ??= currentDateTime.Month;
        Year ??= currentDateTime.Year;
        return this;
    }
    
    public enum FieldOptions
    {
        Amount,
        PaidDateTime
    }
}