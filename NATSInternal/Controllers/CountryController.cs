namespace NATSInternal.Controllers.Api;

[Route("Api/Country")]
[ApiController]
[Authorize]
public class CountryController : ControllerBase
{
    private readonly ICountryService _service;

    public CountryController(ICountryService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CountryList()
    {
        return Ok(await _service.GetListAsync());
    }
}