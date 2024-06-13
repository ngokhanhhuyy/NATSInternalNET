namespace NATSInternal.Controllers.Api;

[Route("/Api/Role")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
public class RoleApiController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleApiController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet("List")]
    public async Task<IActionResult> RoleList()
    {
        return Ok(await _roleService.GetListAsync());
    }
}
