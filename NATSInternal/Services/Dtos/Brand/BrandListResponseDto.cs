namespace NATSInternal.Services.Dtos;

public class BrandListResponseDto
{
    public List<BrandBasicResponseDto> Items { get; set; }
    public BrandListAuthorizationResponseDto Authorization { get; set; }
}
