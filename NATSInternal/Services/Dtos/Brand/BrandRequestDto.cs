namespace NATSInternal.Services.Dtos;

public class BrandRequestDto : IRequestDto<BrandRequestDto>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Website { get; set; }
    public string SocialMediaUrl { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public byte[] ThumbnailFile { get; set; }
    public bool ThumbnailChanged { get; set; }
    public CountryRequestDto Country { get; set; }

    public BrandRequestDto TransformValues()
    {
        Name = Name?.ToNullIfEmpty();
        Website = Website?.ToNullIfEmpty();
        SocialMediaUrl = SocialMediaUrl?.ToNullIfEmpty();
        PhoneNumber = PhoneNumber?.ToNullIfEmpty();
        Email = Email?.ToNullIfEmpty();
        Address = Address?.ToNullIfEmpty();
        Country = Country?.TransformValues();
        return this;
    }
}
