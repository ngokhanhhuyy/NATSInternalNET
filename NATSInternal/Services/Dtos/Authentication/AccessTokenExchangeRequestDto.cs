namespace NATSInternal.Services.Dtos;

public class AccessTokenExchangeRequestDto : IRequestDto<AccessTokenExchangeRequestDto>
{
    public string ExpiredAccessToken { get; set; }
    public string RefreshToken { get; set; }

    public AccessTokenExchangeRequestDto TransformValues()
    {
        ExpiredAccessToken = ExpiredAccessToken?.ToNullIfEmpty();
        RefreshToken = ExpiredAccessToken?.ToNullIfEmpty();
        return this;
    }
}
