namespace NATSInternal.Controllers;

[Route("Api/Customer/{customerId:int}/Debt")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CustomerDebtController : ControllerBase
{
    private readonly IDebtService _service;
    private readonly IValidator<DebtUpsertRequestDto> _upsertValidator;
    
    public CustomerDebtController(
            IDebtService service,
            IValidator<DebtUpsertRequestDto> upsertValidator)
    {
        _service = service;
        _upsertValidator = upsertValidator;
    }
    
    [HttpGet("{debtId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DebtDetail(int customerId, int debtId)
    {
        try
        {
            return Ok(await _service.GetDetailAsync(customerId, debtId));
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpPost]
    [Authorize(Policy = "CanCreateDebt")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DebtCreate(int customerId, [FromBody] DebtUpsertRequestDto requestDto)
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
            int createdId = await _service.CreateAsync(customerId, requestDto);
            string createdResourceUrl = Url.Action("DebtDetail", "CustomerDebt", new { id = createdId });
            return Created(createdResourceUrl, createdId);
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
    
    [HttpPut("{debtId:int}")]
    [Authorize(Policy = "CanEditDebt")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DebtUpdate(
            int customerId,
            int debtId,
            [FromBody] DebtUpsertRequestDto requestDto)
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
            await _service.UpdateAsync(customerId, debtId, requestDto);
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
    
    [HttpDelete("{debtId:int}")]
    [Authorize(Policy = "CanDeleteDebt")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DebtDelete(int customerId, int debtId)
    {
        try
        {
            await _service.DeleteAsync(customerId, debtId);
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
            return UnprocessableEntity(ModelState);
        }
    }
}