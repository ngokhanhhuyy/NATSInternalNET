namespace NATSInternal.Services.Dtos;

public class IntroducerRequestDto : IRequestDto<IntroducerRequestDto>
{
    public int Id { get; set; }

    public IntroducerRequestDto TransformValues()
    {
        return this;
    }
}
