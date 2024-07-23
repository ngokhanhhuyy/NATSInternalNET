namespace NATSInternal.Services.Dtos;

public class SupplyUpsertRequestDto : IRequestDto<SupplyUpsertRequestDto>
{
    public DateTime? PaidDateTime { get; set; }
    public long ShipmentFee { get; set; }
    public string Note { get; set; }
    public string UpdateReason { get; set; }
    public List<SupplyItemRequestDto> Items { get; set; }
    public List<SupplyPhotoRequestDto> Photos { get; set; }

    public SupplyUpsertRequestDto TransformValues()
    {
        Note = Note?.ToNullIfEmpty();
        UpdateReason = UpdateReason?.ToNullIfEmpty();
        Items = Items?.Select(i => i.TransformValues()).ToList();
        Photos = Photos?.Select(p => p.TransformValues()).ToList();
        return this;
    }
}
