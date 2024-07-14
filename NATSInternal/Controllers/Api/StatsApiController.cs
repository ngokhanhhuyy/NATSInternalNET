namespace NATSInternal.Controllers;

[ApiController]
[Route("Api/Stats")]
[Authorize]
public class StatsApiController : ControllerBase
{
    private readonly IStatsService _service;
    private readonly IValidator<MonthlyStatsRequestDto> _monthlyValidator;

    public StatsApiController(
            IStatsService service,
            IValidator<MonthlyStatsRequestDto> monthlyValidator)
    {
        _service = service;
        _monthlyValidator = monthlyValidator;
    }

    [HttpGet("Daily")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DailyStats([FromQuery] DateOnly? recordedDate)
    {
        try
        {
            return Ok(await _service.GetDailyStatsDetailAsync(recordedDate));
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }

    [HttpGet("Monthly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MonthlyStats([FromQuery] MonthlyStatsRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _monthlyValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Fetch the monthly stats.
        try
        {
            return Ok(await _service.GetMonthlyStatsDetailAsync(requestDto));
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(exception);
        }
    }
}