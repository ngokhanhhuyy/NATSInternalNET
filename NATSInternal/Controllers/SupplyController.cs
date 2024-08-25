namespace NATSInternal.Controllers.Api;

[Route("/Api/Supply")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SupplyController : ControllerBase
{
    private readonly ISupplyService _service;
    private readonly IValidator<SupplyListRequestDto> _listValidator;
    private readonly IValidator<SupplyUpsertRequestDto> _upsertValidator;
    private readonly INotifier _notifier;

    public SupplyController(
            ISupplyService service,
            IValidator<SupplyListRequestDto> listValidator,
            IValidator<SupplyUpsertRequestDto> validator,
            INotifier notifier)
    {
        _service = service;
        _listValidator = listValidator;
        _upsertValidator = validator;
        _notifier = notifier;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SupplyList(
            [FromQuery] SupplyListRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _listValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Fetch the list of data.
        return Ok(await _service.GetListAsync(requestDto));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SupplyDetail(int id)
    {
        try
        {
            SupplyDetailResponseDto responseDto = await _service.GetDetailAsync(id);
            return Ok(responseDto);
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }

    [HttpPost]
    [Authorize(Policy = "CanCreateSupply")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> SupplyCreate(
            [FromBody] SupplyUpsertRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult = _upsertValidator
            .Validate(requestDto.TransformValues(), options =>
                options.IncludeRuleSets("Create").IncludeRulesNotInRuleSet());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Perform the creating operation.
        try
        {
            // Create the supply.
            int createdId = await _service.CreateAsync(requestDto);
            string createdResourceUrl = Url.Action(
                "SupplyDetail",
                "Supply",
                new { id = createdId });
            
            // Create and distribute the notification to the users.
            await _notifier.Notify(NotificationType.SupplyCreation, createdId);
            return Created(createdResourceUrl, createdId);
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(ModelState);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "CanEditSupply")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> SupplyUpdate(
            int id,
            [FromBody] SupplyUpsertRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult = _upsertValidator
            .Validate(requestDto.TransformValues(), options =>
                options.IncludeRuleSets("Update").IncludeRulesNotInRuleSet());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Perform the updating operation.
        try
        {
            // Update the supply.
            await _service.UpdateAsync(id, requestDto);
            
            // Create and distribute the notification to the users.
            await _notifier.Notify(NotificationType.SupplyModification, id);
            
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
            return UnprocessableEntity(ModelState);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CanDeleteSupply")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> SupplyDelete(int id)
    {
        try
        {
            // Delete the supply.
            await _service.DeleteAsync(id);
            
            // Create and distribute the notification to the users.
            await _notifier.Notify(NotificationType.SupplyDeletion, id);
            
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
            return UnprocessableEntity(ModelState);
        }
    }
}
