namespace NATSInternal.Controllers;

[Route("Api/Product")]
[ApiController]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;
    private readonly IValidator<ProductUpsertRequestDto> _validator;
    private readonly INotifier _notifier;

    public ProductController(
            IProductService productService,
            IValidator<ProductUpsertRequestDto> upsertValidator,
            INotifier notifier)
    {
        _service = productService;
        _validator = upsertValidator;
        _notifier = notifier;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ProductList(
            [FromQuery] ProductListRequestDto requestDto)
    {
        ProductListResponseDto responseDto;
        responseDto = await _service.GetListAsync(requestDto.TransformValues());
        return Ok(responseDto);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProductDetail(
            int id,
            [FromQuery] ProductDetailRequestDto requestDto)
    {
        try
        {
            ProductDetailResponseDto responseDto;
            responseDto = await _service.GetDetailAsync(id, requestDto);
            return Ok(responseDto);
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }

    [HttpPost]
    [Authorize(Policy = "CanCreateProduct")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ProductCreate(
            [FromBody] ProductUpsertRequestDto requestDto)
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
            // Create the product.
            int createdId = await _service.CreateAsync(requestDto);
            string createdResourceUrl = Url.Action(
                "ProductDetail",
                "Product",
                new { id = createdId });

            // Create and distribute the notification to the users.
            await _notifier.Notify(NotificationType.ProductCreation, createdId);

            return Created(createdResourceUrl, createdId);
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(ModelState);
        }
    }

    [HttpPut("{id:int}")]
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
            // Update the product.
            await _service.UpdateAsync(id, requestDto);

            // Create and distribute the notification to the users.
            await _notifier.Notify(NotificationType.ProductModification, id);

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

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CanDeleteProduct")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProductDelete(int id)
    {
        try
        {
            // Delete the product.
            await _service.DeleteAsync(id);

            // Create and distribute the notification to the users.
            await _notifier.Notify(NotificationType.ProductDeletion, id);

            return NoContent();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }
}