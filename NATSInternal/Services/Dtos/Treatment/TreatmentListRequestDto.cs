namespace NATSInternal.Services.Dtos;

public class TreatmentListRequestDto : IRequestDto<TreatmentListRequestDto>
{
    public bool OrderByAscending { get; set; }
    public string OrderByField { get; set; } = nameof(FieldOptions.PaidDateTime);
    public int? Month { get; set; }
    public int? Year { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;
    
    public TreatmentListRequestDto TransformValues()
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