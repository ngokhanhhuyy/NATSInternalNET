namespace NATSInternal.Services.Dtos;

public class ConsultantListRequestDto :
        IRequestDto<ConsultantListRequestDto>,
        ILockableEntityListRequestDto
{
    public bool OrderByAscending { get; set; }
    public string OrderByField { get; set; } = nameof(FieldOptions.PaidDateTime);
    public int Month { get; set; }
    public int Year { get; set; }
    public bool IgnoreMonthYear { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;

    public ConsultantListRequestDto TransformValues()
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
        PaidDateTime,
        Amount
    }
}