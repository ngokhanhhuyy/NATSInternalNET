namespace NATSInternal.Controller;

[ApiController]
[Route("Api/Consultant")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ConsultantController : ControllerBase
{
    private readonly IConsultantService _consultantService;
    private readonly INotificationService _notificationService;
    private readonly IValidator<ConsultantListRequestDto> _listValidator;
    private readonly IValidator<ConsultantUpsertRequestDto> _upsertValidator;

    public ConsultantController(
            IConsultantService consultantService,
            INotificationService notificationService,
            IValidator<ConsultantListRequestDto> listValidator,
            IValidator<ConsultantUpsertRequestDto> upsertValidator)
    {
        _consultantService = consultantService;
        _notificationService = notificationService;
        _listValidator = listValidator;
        _upsertValidator = upsertValidator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConsultantList(
            [FromQuery] ConsultantListRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _listValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Fetch the data of the list.
        return Ok(await _consultantService.GetListAsync(requestDto));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultantDetail(int id)
    {
        try
        {
            return Ok(await _consultantService.GetDetailAsync(id));
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ConsultantCreate(
            [FromBody] ConsultantUpsertRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _upsertValidator.Validate(
            requestDto.TransformValues(),
            options => options
                .IncludeRuleSets("Create")
                .IncludeRulesNotInRuleSet());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Perform creating operation.
        try
        {
            // Create the consultant.
            int createdId = await _consultantService.CreateAsync(requestDto);
            string createdResourceUrl = Url.Action(
                "ConsultantDetail",
                "Consultant",
                new { id = createdId });
            
            // The consultant has been created successfully, create the notification.
            await _notificationService.CreateAsync(
                NotificationType.ConsultantCreation,
                new List<int> { createdId });

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
        catch (ConcurrencyException)
        {
            return Conflict();
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ConsultantUpdate(
            int id,
            [FromBody] ConsultantUpsertRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _upsertValidator.Validate(
            requestDto.TransformValues(),
            options => options
                .IncludeRuleSets("Update")
                .IncludeRulesNotInRuleSet());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Perform the operations.
        try
        {
            // Perform the update operation.
            await _consultantService.UpdateAsync(id, requestDto);

            // Create the notification.
            await _notificationService.CreateAsync(
                NotificationType.ConsultantModification,
                new List<int> { id });

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
        catch (ConcurrencyException)
        {
            return Conflict();
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ConsultantDelete(int id)
    {
        try
        {
            await _consultantService.DeleteAsync(id);

            // Create the notification.
            await _notificationService.CreateAsync(
                NotificationType.ConsultantDeletion,
                new List<int> { id });
                
            return Ok();
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
        catch (ConcurrencyException)
        {
            return Conflict();
        }
    }
}