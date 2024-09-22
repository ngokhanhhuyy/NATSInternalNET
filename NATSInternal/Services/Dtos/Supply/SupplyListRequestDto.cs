namespace NATSInternal.Services.Dtos;

public class SupplyListRequestDto :
        IRequestDto<SupplyListRequestDto>,
        ILockableEntityListRequestDto
{
    public bool OrderByAscending { get; set; }
    public string OrderByField { get; set; } = nameof(FieldOptions.PaidDateTime);
    public int Year { get; set; }
    public int Month { get; set; }
    public bool IgnoreMonthYear { get; set; } = false;
    public int? UserId { get; set; }
    public int? ProductId { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;

    public SupplyListRequestDto TransformValues()
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
        TotalAmount,
        PaidDateTime,
        ShipmentFee,
        ItemAmount,
    }
}