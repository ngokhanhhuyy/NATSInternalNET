namespace NATSInternal.Services.Dtos;

public class SupplyListRequestDto : IRequestDto<SupplyListRequestDto>
{
    public bool OrderByAscending { get; set; }
    public string OrderByField { get; set; } = nameof(FieldOptions.PaidDateTime);
    public DateOnly? RangeFrom { get; set; }
    public DateOnly? RangeTo { get; set; }
    public int? UserId { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;

    public SupplyListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty();
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