namespace NATSInternal.Controllers;

[Route("Api/DebtIncurrence")]
[ApiController]
[Authorize]
public class DebtIncurrenceController : ControllerBase
{
    private readonly IDebtIncurrenceService _service;
    private readonly IValidator<DebtIncurrenceUpsertRequestDto> _upsertValidator;
    private readonly INotifier _notifier;

    public DebtIncurrenceController(
            IDebtIncurrenceService service,
            IValidator<DebtIncurrenceUpsertRequestDto> upsertValidator,
            INotifier notifier)
    {
        _service = service;
        _upsertValidator = upsertValidator;
        _notifier = notifier;
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DebtIncurrenceDetail(int id)
    {
        try
        {
            return Ok(await _service.GetDetailAsync(id));
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [Authorize(Policy = "CanCreateDebtIncurrence")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DebtIncurrenceCreate(
            [FromBody] DebtIncurrenceUpsertRequestDto requestDto)
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
            // Create the debt incurrence.
            int createdId = await _service.CreateAsync(requestDto);
            string createdUrl = Url.Action(
                "DebtIncurrenceDetail",
                "CustomerDebtIncurrence",
                new { id = createdId });

            // Create and distribute the notification to the users.
            await _notifier.Notify(NotificationType.DebtIncurrenceCreation, createdId);

            return Created(createdUrl, createdId);
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
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "CanEditDebtIncurrence")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DebtIncurrenceUpdate(
            int id,
            [FromBody] DebtIncurrenceUpsertRequestDto requestDto)
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
            // Update the debt incurrence.
            await _service.UpdateAsync(id, requestDto);

            // Create and distribute the notification to the users.
            await _notifier.Notify(NotificationType.DebtIncurrenceModification, id);

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

    [HttpDelete("{debtIncurrenceId:int}")]
    [Authorize(Policy = "CanDeleteDebtIncurrence")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DebtIncurrenceDelete(int id)
    {
        try
        {
            // Delete the debt incurrence.
            await _service.DeleteAsync(id);

            // Create and distribute the notification to the users.
            await _notifier.Notify( NotificationType.DebtIncurrenceDeletion, id);

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
            return UnprocessableEntity(ModelState);
        }
    }
}