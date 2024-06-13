namespace NATSInternal.Controllers.Api;

[Route("Api/Product")]
[ApiController]
[Authorize]
public class ProductApiController : ControllerBase
{
    private readonly IProductService _service;
    private readonly IValidator<ProductUpsertRequestDto> _validator;

    public ProductApiController(
            IProductService productService,
            IValidator<ProductUpsertRequestDto> upsertValidator)
    {
        _service = productService;
        _validator = upsertValidator;
    }

    [HttpGet("List")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ProductList([FromQuery] ProductListRequestDto requestDto)
    {
        ProductListResponseDto responseDto;
        responseDto = await _service.GetListAsync(requestDto.TransformValues());
        return Ok(responseDto);
    }

    [HttpGet("{id:int}/Detail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProductDetail(int id)
    {
        try
        {
            ProductDetailResponseDto responseDto;
            responseDto = await _service.GetDetailAsync(id);
            return Ok(responseDto);
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }

    [HttpPost("Create")]
    [Authorize(Policy = "CanCreateProduct")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ProductCreate([FromBody] ProductUpsertRequestDto requestDto)
    {
        // Validate the data from the request.
        ValidationResult validationResult;
        validationResult = _validator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Performing creating operation.
        try
        {
            int createdId = await _service.CreateAsync(requestDto);
            return Created(Url.Action("ProductDetail", "ProductApi", new { id = createdId }), createdId);
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(ModelState);
        }
    }

    [HttpPut("{id:int}/Update")]
    [Authorize(Policy = "CanEditProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ProductUpdate(
            int id,
            [FromBody] ProductUpsertRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _validator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        // Perform updating operation.
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
    [Authorize(Policy = "CanDeleteProduct")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProductDelete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }
}
