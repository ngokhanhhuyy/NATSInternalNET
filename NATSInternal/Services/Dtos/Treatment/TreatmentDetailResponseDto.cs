namespace NATSInternal.Services.Dtos;

public class TreatmentDetailResponseDto
{
    public int Id { get; set; }
    public DateTime OrderedDateTime { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? LastUpdatedDateTime { get; set; }
    public long ServiceAmount { get; set; }
    public decimal ServiceVatAmount { get; set; }
    public long ProductAmount { get; set; }
    public string Note { get; set; }
    public bool IsClosed { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public UserBasicResponseDto CreatedUser { get; set; }
    public UserBasicResponseDto Therapist { get; set; }
    public List<TreatmentItemResponseDto> Items { get; set; }
    public List<TreatmentPhotoResponseDto> Photos { get; set; }
    public TreatmentAuthorizationResponseDto Authorization { get; set; }

    public TreatmentDetailResponseDto(
            Treatment treatment,
            TreatmentAuthorizationResponseDto authorization)
    {
        Id = treatment.Id;
        OrderedDateTime = treatment.OrderedDateTime;
        LastUpdatedDateTime = treatment.LastUpdatedDateTime;
        ServiceAmount = treatment.ServiceAmount;
        ServiceVatAmount = treatment.ServiceVatFactor;
        ProductAmount = treatment.ProductAmount;
        Note = treatment.Note;
        IsClosed = treatment.IsClosed;
        Customer = new CustomerBasicResponseDto(treatment.Customer);
        CreatedUser = new UserBasicResponseDto(treatment.CreatedUser);
        Therapist = new UserBasicResponseDto(treatment.Therapist);
        Items = treatment.Items?.Select(ti => new TreatmentItemResponseDto(ti)).ToList();
        Photos = treatment.Photos?.Select(tp => new TreatmentPhotoResponseDto(tp)).ToList();
        Authorization = authorization;
    }
}