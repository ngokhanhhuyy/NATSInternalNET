namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle authentication.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Get access token by the provided username and password.
    /// </summary>
    /// <param name="requestDto">An object containing username and password.</param>
    /// <param name="includeExchangeToken">
    /// (Optional) Indicating that the exchange token should be included in the result.
    /// If this parameter is not specified, the default value would be <c>true</c>.
    /// </param>
    /// <returns>
    /// An object containing the access token, the refresh token (if specified to be
    /// included), the userId associated to the tokens and the expiring datetime of
    /// the access token.
    /// </returns>
    /// <exception cref="OperationException">
    /// Thrown when the user with the specified username doesn't exist or the specified
    /// password is incorrect. 
    /// </exception>
    Task<AccessTokenResponseDto> GetAccessTokenAsync(
            SignInRequestDto requestDto,
            bool includeExchangeToken);

    /// <summary>
    /// Get the new fresh access token and refresh token by exchanging the old ones.
    /// </summary>
    /// <param name="requestDto">
    /// An object containing the expired access token and the old refresh token.
    /// </param>
    /// <returns>
    /// An object containing the access token, the refresh token (if specified to be
    /// included), the userId associated to the tokens and the expiring datetime of
    /// the access token.
    /// </returns>
    Task<AccessTokenResponseDto> ExchangeAccessTokenAsync(
            AccessTokenExchangeRequestDto requestDto);

    /// <summary>Sign in with the specified username and password using cookies.</summary>
    /// <param name="requestDto">An object containing username and password.</param>
    /// <returns>
    /// A <c><see cref="int"/></c> representing the id of the signed in user.
    /// </returns>
    Task<int> SignInAsync(SignInRequestDto requestDto);

    /// <summary>
    /// Sign out and clear the cookies which containing the authentication credentials
    /// from the client.
    /// </summary>
    /// <returns>
    /// A <c><see cref="Task"/></c> object representing the asynchronous operation.
    /// </returns>
    Task SignOutAsync();
}
