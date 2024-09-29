namespace NATSInternal.Services.Dtos;

public class ExpenseListRequestDto :
        IRequestDto<ExpenseListRequestDto>,
        ILockableEntityListRequestDto
{
    public bool OrderByAscending { get; set; } = false;
    public string OrderByField { get; set; } = nameof(FieldOptions.PaidDateTime);
    public int Month { get; set; }
    public int Year { get; set; }
    public bool IgnoreMonthYear { get; set; }
    public ExpenseCategory? Category { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;
    
    public ExpenseListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty();

        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        if (!IgnoreMonthYear)
        {
            if (Month == 0)
            {
                Month = currentDateTime.Month;
            }

            if (Year == 0)
            {
                Year = currentDateTime.Year;
            }
        }

        return this;
    }
    
    public enum FieldOptions
    {
        Amount,
        PaidDateTime
    }
}