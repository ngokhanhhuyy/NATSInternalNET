namespace NATSInternal.Controllers.Api;

[Route("/Api/User")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidator<UserListRequestDto> _listValidator;
    private readonly IValidator<UserCreateRequestDto> _createValidator;
    private readonly IValidator<UserUpdateRequestDto> _updateValidator;
    private readonly IValidator<UserPasswordChangeRequestDto> _passwordChangeValidator;
    private readonly IValidator<UserPasswordResetRequestDto> _passwordResetValidator;

    public UserController(
            IUserService userService,
            IValidator<UserListRequestDto> listValidator,
            IValidator<UserCreateRequestDto> createValidator,
            IValidator<UserUpdateRequestDto> updateValidator,
            IValidator<UserPasswordChangeRequestDto> passwordChangeValidator,
            IValidator<UserPasswordResetRequestDto> passwordResetValidator)
    {
        _userService = userService;
        _listValidator = listValidator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _passwordChangeValidator = passwordChangeValidator;
        _passwordResetValidator = passwordResetValidator;

    }

    [HttpGet("List")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UserList([FromQuery] UserListRequestDto requestDto)
    {
        // Validate data from request.
        ValidationResult validationResult;
        validationResult = _listValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Perform fetching operation.
        UserListResponseDto responseDto = await _userService.GetListAsync(requestDto);
        return Ok(responseDto);
    }

    [HttpGet("JoinedRecently")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> JoinedRecentlyUserList()
    {
        UserListResponseDto responseDto = await _userService.GetJoinedRecentlyListAsync();
        return Ok(responseDto);
    }

    [HttpGet("UpcomingBirthday")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpcomingBirthdayUserList()
    {
        UserListResponseDto responseDto = await _userService.GetUpcomingBirthdayListAsync();
        return Ok(responseDto);
    }

    [HttpGet("{id:int}/Role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UserRole(int id)
    {
        try
        {
            return Ok(await _userService.GetRoleAsync(id));
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(exception);
        }
    }

    [HttpGet("{id:int}/Detail")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UserDetail(int id)
    {
        try
        {
            UserDetailResponseDto responseDto;
            responseDto = await _userService.GetDetailAsync(id);
            return Ok(responseDto);
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("Create")]
    [Authorize(Policy = "CanCreateUser")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UserCreate([FromBody] UserCreateRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _createValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Perform creating operation.
        try
        {
            UserCreateResponseDto responseDto = await _userService.CreateAsync(requestDto);
            return Ok(responseDto);
        }
        catch (DuplicatedException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return Conflict(ModelState);
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.Clear();
            ModelState.AddModelError("", exception.Message);
            return UnprocessableEntity(ModelState);
        }
    }

    [HttpPut("{id:int}/Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateRequestDto requestDto)
    {
        ValidationResult validationResult;
        validationResult = _updateValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        try
        {
            await _userService.UpdateAsync(id, requestDto);
            return Ok();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
        catch (AuthorizationException)
        {
            return Forbid();
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(ModelState);
        }
    }

    [HttpPut("{id:int}/ChangePassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult>
        ChangeUserPassword(int id, [FromBody] UserPasswordChangeRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _passwordChangeValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Perform the password change operation.
        try
        {
            await _userService.ChangePasswordAsync(id, requestDto);
            return Ok();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
        catch (AuthorizationException)
        {
            return Forbid();
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(ModelState);
        }
    }

    [HttpPut("{id:int}/ResetPassword")]
    [Authorize(Policy = "CanResetPassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ResetUserPassword(
            int id,
            [FromBody] UserPasswordResetRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _passwordResetValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Perform the password change operation.
        try
        {
            await _userService.ResetPasswordAsync(id, requestDto);
            return Ok();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
        catch (AuthorizationException)
        {
            return Forbid();
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(exception);
        }
    }

    [HttpDelete("{id:int}/Delete")]
    [Authorize(Policy = "CanDeleteUser")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _userService.DeleteAsync(id);
            return Ok();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
        catch (AuthorizationException)
        {
            return Forbid();
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(exception);
        }
    }

    [HttpDelete("{id:int}/Restore")]
    [Authorize(Policy = "CanRestore")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreUser(int id)
    {
        try
        {
            await _userService.RestoreAsync(id);
            return Ok();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
        catch (AuthorizationException)
        {
            return Forbid();
        }
    }
}
