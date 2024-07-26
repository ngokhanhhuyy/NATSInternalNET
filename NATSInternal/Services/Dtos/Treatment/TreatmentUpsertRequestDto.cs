namespace NATSInternal.Services.Dtos;

public class TreatmentUpsertRequestDto : IRequestDto<TreatmentUpsertRequestDto>
{
    public DateTime? OrderedDateTime { get; set; }
    public long ServiceAmount { get; set; }
    public decimal ServiceVatFactor { get; set; }
    public string Note { get; set; }
    public int CustomerId { get; set; }
    public int TherapistId { get; set; }
    public string UpdateReason { get; set; }
    public List<TreatmentItemRequestDto> Items { get; set; }
    public List<TreatmentPhotoRequestDto> Photos { get; set; }

    public TreatmentUpsertRequestDto TransformValues()
    {
        Note = Note?.ToNullIfEmpty();
        Items = Items?.Select(i => i.TransformValues()).ToList();
        Photos = Photos?.Select(p => p.TransformValues()).ToList();
        return this;
    }
}
