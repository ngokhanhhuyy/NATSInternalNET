namespace NATSInternal.Services.Dtos;

public class DebtPaymentListRequestDto :
        IRequestDto<DebtPaymentListRequestDto>,
        ILockableEntityListRequestDto
{
    public bool OrderByAscending { get; set; }
    public string OrderByField { get; set; } = nameof(FieldOptions.PaidDateTime);
    public int Month { get; set; }
    public int Year { get; set; }
    public bool IgnoreMonthYear { get; set; }
    public int? CustomerId { get; set; }
    public int? CreatedUserId { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;

    public DebtPaymentListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty();

        if (!IgnoreMonthYear)
        {
            DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
            Month = Month == 0 ? currentDateTime.Month : Month;
            Year = Year == 0 ? currentDateTime.Year : Year;
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
        PaidDateTime,
        Amount
    }
}
