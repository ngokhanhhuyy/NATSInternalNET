namespace NATSInternal.Services.Dtos;

public class CountryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }

    public CountryResponseDto(Country country)
    {
        Id = country.Id;
        Name = country.Name;
        Code = country.Code;
    }
}
