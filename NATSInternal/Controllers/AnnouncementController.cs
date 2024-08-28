namespace NATSInternal.Controllers;

[Route("/Api/Announcement")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AnnouncementController : ControllerBase
{
    private readonly IAnnouncementService _service;
    private readonly IValidator<AnnouncementListRequestDto> _listValidator;
    private readonly IValidator<AnnouncementUpsertRequestDto> _upsertValidator;
    private readonly INotifier _notifier;

    public AnnouncementController(
            IAnnouncementService service,
            IValidator<AnnouncementListRequestDto> listValidator,
            IValidator<AnnouncementUpsertRequestDto> upsertValidator,
            INotifier notifier)
    {
        _service = service;
        _listValidator = listValidator;
        _upsertValidator = upsertValidator;
        _notifier = notifier;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet]
    public async Task<IActionResult> AnnouncementList(
            [FromQuery] AnnouncementListRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _listValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Fetch the results.
        return Ok(await _service.GetListAsync(requestDto));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> AnnouncementDetail(int id)
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
    [Authorize(Policy = "CanCreateAnnouncement")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AnnouncementCreate(
            [FromBody] AnnouncementUpsertRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult = _upsertValidator
            .Validate(requestDto.TransformValues(), options => options
                .IncludeRuleSets("Create")
                .IncludeRulesNotInRuleSet());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Perform the creating operation.
        try
        {
            // Create the announcement.
            int createdId = await _service.CreateAsync(requestDto);
            string createdResourceUrl = Url.Action(
                "AnnouncementDetail",
                "Announcement",
                new { id = createdId });
            
            // Create and distribute the notification to the users.
            await _notifier.Notify(NotificationType.AnnouncementCreation, createdId);
            
            return Created(createdResourceUrl, createdId);
        }
        catch (ConcurrencyException)
        {
            return Conflict();
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "CanEditAnnouncement")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AnnouncementUpdate(
            int id,
            [FromBody] AnnouncementUpsertRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult = _upsertValidator
            .Validate(requestDto.TransformValues(), options => options
                .IncludeRuleSets("Update")
                .IncludeRulesNotInRuleSet());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Perform update operation.
        try
        {
            // Update the announcement.
            await _service.UpdateAsync(id, requestDto);
            
            // Create and distribute the notification to the users.
            await _notifier.Notify(NotificationType.AnnouncementModification, id);
            
            return Ok();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
        catch (ConcurrencyException)
        {
            return Conflict();
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CanDeleteAnnouncement")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AnnouncementDelete(int id)
    {
        try
        {
            // Delete the announcement.
            await _service.DeleteAsync(id);
            
            // Create and distribute the notification to the users.
            await _notifier.Notify(NotificationType.AnnouncementDeletion, id);
            
            return Ok();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }
}