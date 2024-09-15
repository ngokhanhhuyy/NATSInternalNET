namespace NATSInternal.Services.Interfaces;

/// <summary>
/// A service to handle authentication-related operations.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Retrieves an access token based on the provided username and password.
    /// </summary>
    /// <param name="requestDto">
    /// An instance of the <see cref="SignInRequestDto"/> class, containing the username and
    /// the password of the user to sign in.
    /// </param>
    /// <param name="includeExchangeToken">
    /// (Optional) Indicating that the exchange token should be included in the result.
    /// If this parameter is not specified, the default value would be <c>true</c>.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="AccessTokenResponseDto"/> class, containing the generated
    /// access token, the refresh token (if specified to be included), the id of the user who
    /// is associated to the tokens and the expiring datetime of the access token.
    /// </returns>
    /// <exception cref="OperationException">
    /// Throws when the user with the specified username doesn't exist or the specified
    /// password is incorrect. 
    /// </exception>
    Task<AccessTokenResponseDto> GetAccessTokenAsync(
            SignInRequestDto requestDto,
            bool includeExchangeToken);

    /// <summary>
    /// Retrieves a new fresh access token and refresh token by exchanging the old ones.
    /// </summary>
    /// <param name="requestDto">
    /// An instance of the <see cref="AccessTokenExchangeRequestDto"/> class, containing the
    /// old access token which is assumed to have expired and the exchange token for the
    /// exchanging operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, which result is an
    /// instance of the <see cref="AccessTokenResponseDto"/> class, containing the expired
    /// access token and the old refresh token.
    /// </returns>
    /// <exception cref="OperationException">
    /// Throws under the following circumstances:<br/>
    /// - When the user with the specified id doens't exist or has already been deleted.<br/>
    /// - When the provided refresh token is invalid or expired.
    /// </exception>
    Task<AccessTokenResponseDto> ExchangeAccessTokenAsync(
            AccessTokenExchangeRequestDto requestDto);

    /// <summary>Signs in with the specified username and password using cookies.</summary>
    /// <param name="requestDto">
    /// An instance of the <see cref="SignInRequestDto"/> class, containing the username and
    /// the password for the sign in operation.
    /// </param>
    /// <returns>
    /// A <see cref="int"/> representing the id of the signed in user.
    /// </returns>
    /// <exception cref="OperationException">
    /// Throws under the following circumstances:<br/>
    /// - When the user with the specified username doesn't exist or has already been deleted.
    /// - When the specified password is incorrect.
    /// </exception>
    Task<int> SignInAsync(SignInRequestDto requestDto);

    /// <summary>
    /// Signs out and clear the cookies which contains the authentication credentials from the
    /// requesting user.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task SignOutAsync();
}
