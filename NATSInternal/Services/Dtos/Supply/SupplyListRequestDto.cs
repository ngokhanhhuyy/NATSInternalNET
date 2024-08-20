namespace NATSInternal.Services.Dtos;

public class SupplyListRequestDto : IRequestDto<SupplyListRequestDto>, ILockableEntityListRequestDto
{
    public bool OrderByAscending { get; set; }
    public string OrderByField { get; set; } = nameof(FieldOptions.PaidDateTime);
    public int? Year { get; set; }
    public int? Month { get; set; }
    public int? UserId { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;

    public SupplyListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty();
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        Year ??= currentDateTime.Year;
        Month ??= currentDateTime.Month;
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