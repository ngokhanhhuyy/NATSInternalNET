namespace NATSInternal.Controllers.Api;

[Route("/Api/Customer")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _service;
    private readonly IValidator<CustomerListRequestDto> _listValidator;
    private readonly IValidator<CustomerUpsertRequestDto> _upsertValidator;
    private readonly INotifier _notifier;

    public CustomerController(
            ICustomerService service,
            IValidator<CustomerListRequestDto> listValidator,
            IValidator<CustomerUpsertRequestDto> upsertValidator,
            INotifier notifier)
    {
        _service = service;
        _listValidator = listValidator;
        _upsertValidator = upsertValidator;
        _notifier = notifier;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CustomerList(
            [FromQuery] CustomerListRequestDto requestDto)
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

        // Perform the creating operation.
        try
        {
            // Create the customer.
            int createdId = await _service.CreateAsync(requestDto);
            string createdUrl = Url.Action("CustomerDetail", new { id = createdId });
            
            // Create and distribute the notification to the users.
            await _notifier.Notify(NotificationType.CustomerCreation, createdId);
            
            return Created(createdUrl, createdId);
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

        // Perform the updating operation.
        try
        {
            // Update the customer.
            await _service.UpdateAsync(id, requestDto);
            
            // Create and distribute the notification to the users.
            await _notifier.Notify(NotificationType.CustomerModification, id);
            
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
            // Delete the customer.
            await _service.DeleteAsync(id);
            
            // Create and distribute the notification to the users.
            await _notifier.Notify(NotificationType.CustomerDeletion, id);
            
            return Ok();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }
}
