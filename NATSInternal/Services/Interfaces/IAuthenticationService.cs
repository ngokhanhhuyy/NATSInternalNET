namespace NATSInternal.Services.Interfaces;

public interface IAuthenticationService
{
    Task<AccessTokenResponseDto> GetAccessTokenAsync(SignInRequestDto requestDto);

    Task<AccessTokenResponseDto> ExchangeAccessTokenAsync(
            AccessTokenExchangeRequestDto requestDto);

    Task SignInAsync(SignInRequestDto requestDto);

    Task SignOutAsync();
}
