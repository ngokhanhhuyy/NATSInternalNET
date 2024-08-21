namespace NATSInternal.Controllers.Api;

[Route("/Api/Customer")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _service;
    private readonly IValidator<CustomerListRequestDto> _listValidator;
    private readonly IValidator<CustomerUpsertRequestDto> _upsertValidator;

    public CustomerController(
            ICustomerService service,
            IValidator<CustomerListRequestDto> listValidator,
            IValidator<CustomerUpsertRequestDto> upsertValidator)
    {
        _service = service;
        _listValidator = listValidator;
        _upsertValidator = upsertValidator;
    }

    [HttpGet]
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
        responseDto = await _service.GetListAsync(requestDto);
        return Ok(responseDto);
    }

    [HttpGet("{id:int}/Basic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CustomerBasic(int id)
    {
        try
        {
            CustomerBasicResponseDto responseDto = await _service.GetBasicAsync(id);
            return Ok(responseDto);
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "CanGetCustomerDetail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CustomerDetail(int id)
    {
        try
        {
            CustomerDetailResponseDto responseDto = await _service.GetDetailAsync(id);
            return Ok(responseDto);
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }

    [HttpPost]
    [Authorize(Policy = "CanCreateCustomer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
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
            int createdCustomerId = await _service.CreateAsync(requestDto);
            string createdCustomerUrl = Url.Action("CustomerDetail", new { id = createdCustomerId });
            return Created(createdCustomerUrl, createdCustomerId);
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }

    [HttpPut("{id:int}")]
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
            await _service.UpdateAsync(id, requestDto);
            return Ok();
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(ModelState);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CanDeleteCustomer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CustomerDelete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }
}
