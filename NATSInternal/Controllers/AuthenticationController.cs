namespace NATSInternal.Controllers;

[Route("/Api/Authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IValidator<SignInRequestDto> _signInValidator;

    public AuthenticationController(
            IAuthenticationService authenticationService,
            IValidator<SignInRequestDto> signInValidator)
    {
        _authenticationService = authenticationService;
        _signInValidator = signInValidator;
    }

    [HttpPost("GetAccessToken")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetAccessToken([FromBody] SignInRequestDto requestDto)
    {
        // Validate data from request.
        ValidationResult validationResult;
        validationResult = _signInValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Performing login request verification operation.
        try
        {
            AccessTokenResponseDto responseDto;
            responseDto = await _authenticationService.GetAccessTokenAsync(requestDto);
            return Ok(responseDto);
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(ModelState);
        }
    }

    [HttpPost("ExchangeAccessToken")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetRefreshToken(
        [FromBody] AccessTokenExchangeRequestDto requestDto)
    {
        try
        {
            AccessTokenResponseDto responseDto;
            responseDto = await _authenticationService.ExchangeAccessTokenAsync(requestDto);
            return Ok(responseDto);
        }
        catch (OperationException exception)
        {
            return UnprocessableEntity(exception.Message);
        }
    }

}
