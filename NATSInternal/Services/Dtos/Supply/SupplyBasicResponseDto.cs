namespace NATSInternal.Services.Dtos;

public class SupplyBasicResponseDto
{
    public int Id { get; set; }
    public DateTime SuppliedDateTime { get; set; }
    public long TotalAmount { get; set; }
    public bool IsClosed { get; set; }
    public UserBasicResponseDto User { get; set; }
    public string FirstPhotoUrl { get; set; }
    public SupplyAuthorizationResponseDto Authorization { get; set; }

    public SupplyBasicResponseDto(Supply supply)
    {
        MapFromEntity(supply);
    }

    public SupplyBasicResponseDto(Supply supply, SupplyAuthorizationResponseDto authorization)
    {
        MapFromEntity(supply);
        Authorization = authorization;
    }

    private void MapFromEntity(Supply supply)
    {
        Id = supply.Id;
        SuppliedDateTime = supply.PaidDateTime;
        TotalAmount = supply.TotalAmount;
        IsClosed = supply.IsClosed;
        User = new UserBasicResponseDto(supply.CreatedUser);
        FirstPhotoUrl = supply.FirstPhotoUrl;
    }
}
