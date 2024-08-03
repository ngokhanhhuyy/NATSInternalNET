namespace NATSInternal.Services.Dtos;

public class DebtPaymentListRequestDto : IRequestDto<DebtPaymentListRequestDto>
{
    public bool OrderByAscending { get; set; }
    public string OrderByField { get; set; } = nameof(FieldOptions.PaidDateTime);
    public DateOnly? RangeFrom { get; set; }
    public DateOnly? RangeTo { get; set; }
    public int Page { get; set; }
    public int ResultsPerPage { get; set; }

    public DebtPaymentListRequestDto TransformValues()
    {
        return this;
    }

    public enum FieldOptions
    {
        PaidDateTime,
        Amount
    }
}