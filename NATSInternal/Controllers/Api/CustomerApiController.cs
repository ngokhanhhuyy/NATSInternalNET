namespace NATSInternal.Controllers.Api;

[Route("/Api/Customer")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CustomerApiController : ControllerBase
{
    private ICustomerService _customerService;
    private IValidator<CustomerListRequestDto> _listValidator;
    private IValidator<CustomerUpsertRequestDto> _upsertValidator;

    public CustomerApiController(
            ICustomerService customerService,
            IValidator<CustomerListRequestDto> listValidator,
            IValidator<CustomerUpsertRequestDto> upsertValidator)
    {
        _customerService = customerService;
        _listValidator = listValidator;
        _upsertValidator = upsertValidator;
    }

    [HttpGet("List")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CustomerList([FromQuery] CustomerListRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _listValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Call service for data fetching.
        CustomerListResponseDto responseDto;
        responseDto = await _customerService.GetListAsync(requestDto);
        return Ok(responseDto);
    }

    [HttpGet("{id:int}/Basic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CustomerBasic(int id)
    {
        CustomerBasicResponseDto responseDto;
        try
        {
            responseDto = await _customerService.GetBasicAsync(id);
            return Ok(responseDto);
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }

    [HttpGet("{id:int}/Detail")]
    [Authorize(Policy = "CanGetCustomerDetail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CustomerDetail(int id)
    {
        CustomerDetailResponseDto responseDto;
        try
        {
            responseDto = await _customerService.GetDetailAsync(id);
            return Ok(responseDto);
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }

    [HttpPost("Create")]
    [Authorize(Policy = "CanCreateCustomer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CustomerCreate(
            [FromBody] CustomerUpsertRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _upsertValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Call service for creating operation.
        try
        {
            CustomerCreateResponseDto responseDto;
            responseDto = await _customerService.CreateAsync(requestDto);
            return Ok(responseDto);
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }

    [HttpPut("{id:int}/Update")]
    [Authorize(Policy = "CanEditCustomer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CustomerUpdate(
            int id,
            [FromBody] CustomerUpsertRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _upsertValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Call service for updating operation.
        try
        {
            await _customerService.UpdateAsync(id, requestDto);
            return Ok();
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(ModelState);
        }
    }

    [HttpDelete("{id:int}/Delete")]
    [Authorize(Policy = "CanDeleteCustomer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CustomerDelete(int id)
    {
        try
        {
            await _customerService.DeleteAsync(id);
            return Ok();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }

}
