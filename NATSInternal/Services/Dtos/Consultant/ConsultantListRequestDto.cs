namespace NATSInternal.Services.Dtos;

public class ConsultantListRequestDto : IRequestDto<ConsultantListRequestDto>, ILockableEntityListRequestDto
{
    public bool OrderByAscending { get; set; }
    public string OrderByField { get; set; } = nameof(FieldOptions.PaidDateTime);
    public int? Month { get; set; }
    public int? Year { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;

    public ConsultantListRequestDto TransformValues()
    {
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        Month ??= currentDateTime.Month;
        Year ??= currentDateTime.Year;
        return this;
    }

    public enum FieldOptions
    {
        PaidDateTime,
        Amount
    }
}