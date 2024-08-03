namespace NATSInternal.Services.Dtos;

public class TreatmentDetailResponseDto
{
    public int Id { get; set; }
    public DateTime PaidDateTime { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? LastUpdatedDateTime { get; set; }
    public long ServiceAmount { get; set; }
    public long ServiceVatAmount { get; set; }
    public decimal ServiceVatFactor { get; set; }
    public long ProductAmount { get; set; }
    public long TotalAmountAfterVAT { get; set; }
    public string Note { get; set; }
    public bool IsLocked { get; set; }
    public CustomerBasicResponseDto Customer { get; set; }
    public UserBasicResponseDto CreatedUser { get; set; }
    public UserBasicResponseDto Therapist { get; set; }
    public List<TreatmentItemResponseDto> Items { get; set; }
    public List<TreatmentPhotoResponseDto> Photos { get; set; }
    public TreatmentAuthorizationResponseDto Authorization { get; set; }
    public List<TreatmentUpdateHistoryResponseDto> UpdateHistories { get; set; }

    public TreatmentDetailResponseDto(
            Treatment treatment,
            TreatmentAuthorizationResponseDto authorization,
            bool mapUpdateHistories = false)
    {
        Id = treatment.Id;
        PaidDateTime = treatment.PaidDateTime;
        LastUpdatedDateTime = treatment.LastUpdatedDateTime;
        ServiceAmount = treatment.ServiceAmount;
        ServiceVatAmount = treatment.ServiceVatAmount;
        ServiceVatFactor = treatment.ServiceVatFactor;
        ProductAmount = treatment.ProductAmount;
        TotalAmountAfterVAT = treatment.TotalAmountAfterVAT;
        Note = treatment.Note;
        IsLocked = treatment.IsLocked;
        Customer = new CustomerBasicResponseDto(treatment.Customer);
        CreatedUser = new UserBasicResponseDto(treatment.CreatedUser);
        Therapist = new UserBasicResponseDto(treatment.Therapist);
        Items = treatment.Items?.Select(ti => new TreatmentItemResponseDto(ti)).ToList();
        Photos = treatment.Photos?.Select(tp => new TreatmentPhotoResponseDto(tp)).ToList();
        Authorization = authorization;

        if (mapUpdateHistories)
        {
            UpdateHistories = treatment.UpdateHistories
                .Select(uh => new TreatmentUpdateHistoryResponseDto(uh))
                .ToList();
        }
    }
}