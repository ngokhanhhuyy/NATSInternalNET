namespace NATSInternal.Services.Dtos;

public class BrandListResponseDto
{
    public List<BrandBasicResponseDto> Items { get; set; }
    public BrandAuthorizationResponseDto Authorization { get; set; }
}
