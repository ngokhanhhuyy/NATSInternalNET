namespace NATSInternal.Controllers;

[Route("Api/Customer/{customerId:int}/DebtPayment")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CustomerDebtPaymentController : ControllerBase
{
    private readonly IDebtPaymentService _service;
    private readonly IValidator<DebtPaymentUpsertRequestDto> _upsertValidator;
    private readonly INotifier _notifier;

    public CustomerDebtPaymentController(
            IDebtPaymentService service,
            IValidator<DebtPaymentUpsertRequestDto> upsertValidator,
            INotifier notifier)
    {
        _service = service;
        _upsertValidator = upsertValidator;
        _notifier = notifier;
    }
    
    [HttpGet("{debtPaymentId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DebtPaymentDetail(
            int customerId,
            int debtPaymentId)
    {
        try
        {
            return Ok(await _service.GetDetailAsync(customerId, debtPaymentId));
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpPost]
    [Authorize(Policy = "CanCreateDebtPayment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DebtPaymentCreate(
            int customerId,
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
        
        // Perform the creating operation.
        try
        {
            // Create the debt payment.
            int createdId = await _service.CreateAsync(customerId, requestDto);
            string createdResourceUrl = Url.Action(
                "DebtPaymentDetail",
                "CustomerDebtPayment",
                new { id = createdId });
            
            // Create and distribute the notification to the users.
            await _notifier.Notify(
                NotificationType.DebtPaymentCreation,
                customerId,
                createdId);
            
            return Created(createdResourceUrl, createdId);
        }
        catch (AuthorizationException)
        {
            return Forbid();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(ModelState);
        }
    }
    
    [HttpPut("{debtPaymentId:int}")]
    [Authorize(Policy = "CanEditDebtPayment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DebtPaymentUpdate(
            int customerId,
            int debtPaymentId,
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
            // Update the debt payment.
            await _service.UpdateAsync(customerId, debtPaymentId, requestDto);
            
            // Create and distribute the notification to the users.
            await _notifier.Notify(
                NotificationType.DebtPaymentModification,
                customerId,
                debtPaymentId);
            
            return Ok();
        }
        catch (AuthorizationException)
        {
            return Forbid();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
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
    
    [HttpDelete("{debtPaymentId:int}")]
    [Authorize(Policy = "CanDeleteDebtPayment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DebtPaymentDelete(int customerId, int debtPaymentId)
    {
        try
        {
            // Delete the debt payment.
            await _service.DeleteAsync(customerId, debtPaymentId);
            
            // Create and distribute the notification to the users.
            await _notifier.Notify(
                NotificationType.DebtPaymentDeletion,
                customerId,
                debtPaymentId);
            
            return Ok();
        }
        catch (AuthorizationException)
        {
            return Forbid();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
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