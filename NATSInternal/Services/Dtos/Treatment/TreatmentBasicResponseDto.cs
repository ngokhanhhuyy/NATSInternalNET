namespace NATSInternal.Services.Dtos;

public class TreatmentBasicResponseDto
{
    public int Id { get; set; }
    public DateTime OrderedDateTime { get; set; }
    public long Amount { get; set; }
    public bool IsClosed { get; set; }
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
        OrderedDateTime = treatment.OrderedDateTime;
        Amount = treatment.Amount;
        IsClosed = treatment.IsClosed;
        Customer = new CustomerBasicResponseDto(treatment.Customer);
    }
}