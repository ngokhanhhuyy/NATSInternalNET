namespace NATSInternal.Controllers;

[Route("Api/Debt")]
[ApiController]
[Authorize]
public class DebtController : ControllerBase
{
    private readonly IDebtService _service;
    private readonly IValidator<DebtListRequestDto> _listValidator;
    private readonly IValidator<DebtUpsertRequestDto> _upsertValidator;
    
    public DebtController(
            IDebtService service,
            IValidator<DebtListRequestDto> listValidator,
            IValidator<DebtUpsertRequestDto> upsertValidator)
    {
        _service = service;
        _listValidator = listValidator;
        _upsertValidator = upsertValidator;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DebtList([FromQuery] DebtListRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _listValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }
        
        // Fetch data.
        return Ok(await _service.GetListAsync(requestDto));
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DebtDetail(int id)
    {
        try
        {
            return Ok(await _service.GetDetailAsync(id));
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DebtCreate(DebtUpsertRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _upsertValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }
        
        // Perform the creating operation.
        try
        {
            int createdId = await _service.CreateAsync(requestDto);
            string createdResourceUrl = Url.Action("DebtDetail", "")
        }
    }
    
}