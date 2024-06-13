namespace NATSInternal.Services.Dtos;

public class SupplyDetailResponseDto
{
    public int Id { get; set; }
    public DateTime SuppliedDateTime { get; set; }
    public long ShipmentFee { get; set; }
    public long PaidAmount { get; set; }
    public string Note { get; set; }
    public List<SupplyItemResponseDto> Items { get; set; }
    public List<SupplyPhotoResponseDto> Photos { get; set; }
    public UserBasicResponseDto User { get; set; }
}
