namespace NATSInternal.Services.Dtos;

public class TreatmentUpdateHistoryDataDto
{
    public DateTime PaidDateTime { get; set; }
    public long ServiceAmount { get; set; }
    public decimal ServiceVatFactor { get; set; }
    public string Note { get; set; }
    public int TherapistId { get; set; }
    public int CustomerId { get; set; }
    
    public TreatmentUpdateHistoryDataDto(Treatment treatment)
    {
        PaidDateTime = treatment.PaidDateTime;
        ServiceAmount = treatment.ServiceAmount;
        ServiceVatFactor = treatment.ServiceVatFactor;
        Note = treatment.Note;
        TherapistId = treatment.TherapistId;
        CustomerId = treatment.CustomerId;
    }
}