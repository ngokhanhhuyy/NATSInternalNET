namespace NATSInternal.Services.Dtos;

public class SupplyDetailResponseDto
{
    public int Id { get; set; }
    public DateTime SuppliedDateTime { get; set; }
    public long ShipmentFee { get; set; }
    public long ItemAmount { get; set; }
    public long TotalAmount { get; set; }
    public string Note { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public bool IsClosed { get; set; }
    public List<SupplyItemResponseDto> Items { get; set; }
    public List<SupplyPhotoResponseDto> Photos { get; set; }
    public UserBasicResponseDto User { get; set; }
    public SupplyAuthorizationResponseDto Authorization { get; set; }

    public SupplyDetailResponseDto(Supply supply, SupplyAuthorizationResponseDto authorization)
    {
        Id = supply.Id;
        SuppliedDateTime = supply.SuppliedDateTime;
        ShipmentFee = supply.ShipmentFee;
        ItemAmount = supply.ItemAmount;
        TotalAmount = supply.TotalAmount;
        Note = supply.Note;
        IsClosed = supply.IsClosed;
        CreatedDateTime = supply.CreatedDateTime;
        UpdatedDateTime = supply.UpdatedDateTime;
        Items = supply.Items?
            .OrderBy(i => i.Id)
            .Select(i => new SupplyItemResponseDto(i)).ToList();
        Photos = supply.Photos?
            .OrderBy(p => p.Id)
            .Select(p => new SupplyPhotoResponseDto(p)).ToList();
        User = new UserBasicResponseDto(supply.CreatedUser);
        Authorization = authorization;
    }
}
