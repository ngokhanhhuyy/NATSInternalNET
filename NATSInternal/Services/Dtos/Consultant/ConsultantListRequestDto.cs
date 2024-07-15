namespace NATSInternal.Services.Dtos;

public class ConsultantListRequestDto : IRequestDto<ConsultantListRequestDto>
{
    public bool OrderByAscending { get; set; }
    public string OrderByField { get; set; }
    public DateOnly? RangeFrom { get; set; }
    public DateOnly? RangeTo { get; set; }
    public int Page { get; set; }
    public int ResultsPerPage { get; set; }

    public ConsultantListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty();
        return this;
    }

    public enum FieldOptions
    {
        PaidDateTime,
        Amount
    }
}