namespace NATSInternal.Services.Dtos;

public class DebtListRequestDto : IRequestDto<DebtListRequestDto>, ILockableEntityListRequestDto
{
    public bool OrderByAscending { get; set; }
    public string OrderByField { get; set; } = nameof(FieldOptions.IncurredDateTime);
    public int? Month { get; set; }
    public int? Year { get; set; } = DateTime.UtcNow.ToApplicationTime().Year;
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;
    
    public DebtListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty();
        Month ??= DateTime.UtcNow.ToApplicationTime().Year;
        Year ??= DateTime.UtcNow.ToApplicationTime().Month;
        return this;
    }
    
    public enum FieldOptions
    {
        IncurredDateTime,
        Amount
    }
}
