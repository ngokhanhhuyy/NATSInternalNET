namespace NATSInternal.Controllers;

[Route("Api/DebtPayment")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CustomerDebtPaymentController : ControllerBase
{
    private readonly IDebtPaymentService _service;
    private readonly IValidator<DebtPaymentListRequestDto> _listValidator;
    private readonly IValidator<DebtPaymentUpsertRequestDto> _upsertValidator;

    public CustomerDebtPaymentController(
            IDebtPaymentService service,
            IValidator<DebtPaymentListRequestDto> listValidator,
            IValidator<DebtPaymentUpsertRequestDto> upsertValidator)
    {
        _service = service;
        _listValidator = listValidator;
        _upsertValidator = upsertValidator;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DebtPaymentList([FromQuery] DebtPaymentListRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _listValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }
        
        // Fetch data.
        return Ok(await _service.GetListAsync(requestDto));
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DebtPaymentDetail(int id)
    {
        try
        {
            return Ok(await _service.GetDetailAsync(id));
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }
    
    [HttpPost]
    [Authorize(Policy = "CanCreateDebtPayment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DebtPaymentCreate([FromBody] DebtPaymentUpsertRequestDto requestDto)
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
            int createdId = await _service.CreateAsync(requestDto);
            string createdResourceUrl = Url.Action(
                "DebtPaymentDetail",
                "CustomerDebtPayment",
                new { id = createdId });
            return Created(createdResourceUrl, createdId);
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
    
    [HttpPut("{id:int}")]
    [Authorize(Policy = "CanEditDebtPayment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DebtPaymentUpdate(
            int id,
            [FromBody] DebtPaymentUpsertRequestDto requestDto)
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
            await _service.UpdateAsync(id, requestDto);
            return Ok();
        }
        catch (AuthorizationException)
        {
            return Forbid();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(exception);
        }
        catch (ConcurrencyException)
        {
            return Conflict();
        }
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CanDeleteDebtPayment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DebtPaymentDelete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
        catch (AuthorizationException)
        {
            return Forbid();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
        catch (ConcurrencyException)
        {
            return Conflict();
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(exception);
        }
    }
}