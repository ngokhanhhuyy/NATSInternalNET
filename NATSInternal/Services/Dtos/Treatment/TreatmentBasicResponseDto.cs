namespace NATSInternal.Services.Dtos;

public class TreatmentBasicResponseDto
{
    public int Id { get; set; }
    public DateTime PaidDateTime { get; set; }
    public long Amount { get; set; }
    public bool IsLocked { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public TreatmentAuthorizationResponseDto Authorization { get; set; }
    
    public TreatmentBasicResponseDto(Treatment treatment)
    {
        MapFromEntity(treatment);
    }

    public TreatmentBasicResponseDto(
            Treatment treatment,
            TreatmentAuthorizationResponseDto authorizationResponseDto)
    {
        MapFromEntity(treatment);
        Authorization = authorizationResponseDto;
    }

    private void MapFromEntity(Treatment treatment)
    {
        Id = treatment.Id;
        PaidDateTime = treatment.PaidDateTime;
        Amount = treatment.Amount;
        IsLocked = treatment.IsLocked;
        Customer = new CustomerBasicResponseDto(treatment.Customer);
    }
}