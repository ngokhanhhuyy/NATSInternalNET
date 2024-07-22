namespace NATSInternal.Services.Dtos;

public class BrandBasicResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public BrandAuthorizationResponseDto Authorization { get; set; }

    public BrandBasicResponseDto(Brand brand)
    {
        MapFromEntity(brand);
    }

    public BrandBasicResponseDto(Brand brand, BrandAuthorizationResponseDto authorization)
    {
        MapFromEntity(brand);
        Authorization = authorization;
    }

    private void MapFromEntity(Brand brand)
    {
        Id = brand.Id;
        Name = brand.Name;
    }
}
