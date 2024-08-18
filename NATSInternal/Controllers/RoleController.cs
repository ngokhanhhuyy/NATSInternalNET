namespace NATSInternal.Controllers.Api;

[Route("/Api/Role")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> RoleList()
    {
        return Ok(await _roleService.GetListAsync());
    }
}
