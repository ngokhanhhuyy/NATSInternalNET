namespace NATSInternal.Services.Dtos;

public class CountryRequestDto : IRequestDto<CountryRequestDto>
{
    public int Id { get; set; }

    public CountryRequestDto TransformValues()
    {
        return this;
    }
}
