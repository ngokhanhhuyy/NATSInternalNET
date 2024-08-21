namespace NATSInternal.Controllers;

[Route("/Api/Announcement")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AnnouncementController : ControllerBase
{
    private readonly IAnnouncementService _service;
    private readonly IValidator<AnnouncementListRequestDto> _listValidator;
    private readonly IValidator<AnnouncementUpsertRequestDto> _upsertValidator;

    public AnnouncementController(
            IAnnouncementService service,
            IValidator<AnnouncementListRequestDto> listValidator,
            IValidator<AnnouncementUpsertRequestDto> upsertValidator)
    {
        _service = service;
        _listValidator = listValidator;
        _upsertValidator = upsertValidator;
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
    
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
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
            int createdAnnouncementId = await _service.CreateAsync(requestDto);
            string createdAnnouncementUrl = Url.Action(
                "AnnouncementDetail",
                "Announcement",
                new { id = createdAnnouncementId });
            return Created(createdAnnouncementUrl, createdAnnouncementId);
        }
        catch (ConcurrencyException)
        {
            return Conflict();
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPut("{id:int}")]
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
            await _service.UpdateAsync(id, requestDto);
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

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> AnnouncementDelete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }
}