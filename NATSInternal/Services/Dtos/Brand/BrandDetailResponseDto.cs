namespace NATSInternal.Services.Dtos;

public class BrandDetailResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Website { get; set; }
    public string SocialMediaUrl { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string ThumbnailUrl { get; set; }
    public CountryResponseDto Country { get; set; }
    public BrandAuthorizationResponseDto Authorization { get; set; }

    public BrandDetailResponseDto(Brand brand, BrandAuthorizationResponseDto authorization)
    {
        Id = brand.Id;
        Name = brand.Name;
        Website = brand.Website;
        SocialMediaUrl = brand.SocialMediaUrl;
        PhoneNumber = brand.PhoneNumber;
        Email = brand.Email;
        Address = brand.Address;
        ThumbnailUrl = brand.ThumbnailUrl;
        Authorization = authorization;

        if (brand.Country != null)
        {
            Country = new CountryResponseDto(brand.Country);
        }
    }
}
