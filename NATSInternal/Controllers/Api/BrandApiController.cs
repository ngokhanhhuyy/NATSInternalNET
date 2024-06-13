namespace NATSInternal.Controllers.Api;

[Route("Api/Brand")]
[ApiController]
[Authorize]
public class BrandApiController : ControllerBase
{
    private readonly IBrandService _service;
    private readonly IValidator<BrandRequestDto> _validator;

    public BrandApiController(
            IBrandService service,
            IValidator<BrandRequestDto> validator)
    {
        _service = service;
        _validator = validator;
    }

    [HttpGet("List")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> BrandList()
    {
        return Ok(await _service.GetListAsync());
    }

    [HttpGet("{id:int}/Detail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BrandDetail(int id)
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

    [HttpPost("Create")]
    [Authorize(Policy = "CanCreateBrand")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> BrandCreate([FromBody] BrandRequestDto requestDto)
    {
        ValidationResult validationResult;
        validationResult = _validator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        try
        {
            int createdBrandId = await _service.CreateAsync(requestDto);
            string createdResourceUrl = Url.Action("Detail", "BrandApi", new { id = createdBrandId });
            return Created(createdResourceUrl, createdBrandId);
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(ModelState);
        }
    }

    [HttpPut("{id:int}/Update")]
    [Authorize(Policy = "CanEditBrand")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> BrandUpdate(int id, [FromBody] BrandRequestDto requestDto)
    {
        ValidationResult validationResult;
        validationResult = _validator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        try
        {
            await _service.UpdateAsync(id, requestDto);
            return Ok();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(ModelState);
        }
    }

    [HttpDelete("{id:int}/Delete")]
    [Authorize(Policy = "CanDeleteBrand")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> BrandDelete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }
}
