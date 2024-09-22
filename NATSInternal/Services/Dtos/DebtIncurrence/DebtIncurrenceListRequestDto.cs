namespace NATSInternal.Services.Dtos;

public class DebtIncurrenceListRequestDto :
        IRequestDto<DebtIncurrenceListRequestDto>,
        ILockableEntityListRequestDto
{
    public bool OrderByAscending { get; set; }
    public string OrderByField { get; set; } = nameof(FieldOptions.IncurredDateTime);
    public int? Month { get; set; }
    public int? Year { get; set; }
    public bool IgnoreMonthYear { get; set; }
    public int? CustomerId  { get; set; }
    public int? CreatedUserId { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;

    public DebtIncurrenceListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty();

        if (!IgnoreMonthYear)
        {
            DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
            Month ??= currentDateTime.Month;
            Year ??= currentDateTime.Year;
        }
        
        if (CustomerId == 0)
        {
            CustomerId = null;
        }

        if (CreatedUserId == 0)
        {
            CreatedUserId = null;
        }
        return this;
    }

    public enum FieldOptions
    {
        IncurredDateTime,
        Amount
    }
}