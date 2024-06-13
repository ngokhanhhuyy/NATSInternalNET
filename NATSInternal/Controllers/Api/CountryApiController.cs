namespace NATSInternal.Controllers.Api;

[Route("Api/Country")]
[ApiController]
[Authorize]
public class CountryApiController : ControllerBase
{
    private readonly ICountryService _service;

    public CountryApiController(ICountryService service)
    {
        _service = service;
    }

    [HttpGet("List")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CountryList()
    {
        return Ok(await _service.GetListAsync());
    }
}
