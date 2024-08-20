namespace NATSInternal.Controllers;

[Route("Api/HealthCheck")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    private readonly IStatsTaskService _statsTaskService;

    public HealthCheckController(IStatsTaskService statsTaskService)
    {
        _statsTaskService = statsTaskService;
    }

    [HttpGet("Ping")]
    public IActionResult Ping()
    {
        return Ok();
    }

    [HttpGet("StartedDateTime")]
    public IActionResult StartedDateTime()
    {
        return Ok(_statsTaskService.StartedDateTime);
    }

    [HttpGet("ExpectedRestartingDateTime")]
    public IActionResult ExpectedRestartingDateTime()
    {
        return Ok(_statsTaskService.ExpectedRestartingDateTime);
    }

    [HttpGet("RunningTime")]
    public IActionResult RunningTime()
    {
        return Ok(_statsTaskService.RunningTime);
    }

    [HttpGet("RemainingTime")]
    public IActionResult RemainingTime()
    {
        return Ok(_statsTaskService.RemainingTime);
    }
}