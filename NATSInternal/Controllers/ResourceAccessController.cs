namespace NATSInternal.Controllers;

[Route("Api/ResourceAccess")]
[ApiController]
[Authorize]
public class ResourceAccessController : ControllerBase
{
    private readonly IUserService _userService;

    public ResourceAccessController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("AccessingUsers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AccessingUser([FromQuery] Resource resource)
    {
        // Get the ids of the users who are accessing the specified resource.
        List<int> userIds;
        if (resource != null)
        {
            userIds = ApplicationHub.GetUserIdsConnectingToResource(resource);
        }
        else
        {
            userIds = ApplicationHub.ConnectingUserIds;
        }

        // Fetch the list of users.
        try
        {
            return Ok(await _userService.GetListAsync(userIds));
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }
}