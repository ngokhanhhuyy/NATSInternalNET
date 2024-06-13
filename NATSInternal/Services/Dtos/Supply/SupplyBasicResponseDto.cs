namespace NATSInternal.Services.Dtos;

public class SupplyBasicResponseDto
{
    public int Id { get; set; }
    public DateTime SuppliedDateTime { get; set; }
    public long TotalAmount { get; set; }
    public UserBasicResponseDto User { get; set; }
    public string FirstPhotoUrl { get; set; }
}
