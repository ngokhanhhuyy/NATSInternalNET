namespace NATSInternal.Services.Dtos;

public class OrderListRequestDto : IRequestDto<OrderListRequestDto>
{
    public bool OrderByAscending { get; set; }
    public string OrderByField { get; set; } = nameof(FieldOptions.OrderedDateTime);
    public DateOnly? RangeFrom { get; set; }
    public DateOnly? RangeTo { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;
        
    public OrderListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty();
        return this;
    }
        
    public enum FieldOptions
    {
        OrderedDateTime,
        Amount
    }
}