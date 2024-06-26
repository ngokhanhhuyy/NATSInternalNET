﻿namespace NATSInternal.Controllers.Api;

[Route("Api/ProductCategory")]
[ApiController]
[Authorize]
public class ProductCategoryApiController : ControllerBase
{
    private readonly IProductCategoryService _service;
    private readonly IValidator<ProductCategoryRequestDto> _validator;

    public ProductCategoryApiController(
            IProductCategoryService service,
            IValidator<ProductCategoryRequestDto> validator)
    {
        _service = service;
        _validator = validator;
    }

    [HttpGet("List")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ProductCategoryList()
    {
        return Ok(await _service.GetListAsync());
    }

    [HttpGet("{id:int}/Detail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProductCategoryDetail(int id)
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
    [Authorize(Policy = "CanCreateProductCategory")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ProductCategoryCreate(
            [FromBody] ProductCategoryRequestDto requestDto)
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
            int createdId = await _service.CreateAsyns(requestDto);
            string createdResourceUrl = Url.Action(
                "ProductCategoryDetail",
                "ProductCategoryApi",
                new { id = createdId });
            return Created(createdResourceUrl, createdId);
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(exception);
        }
    }

    [HttpPut("{id:int}/Update")]
    [Authorize(Policy = "CanEditProductCategory")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ProductCategoryUpdate(
            int id,
            [FromBody] ProductCategoryRequestDto requestDto)
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
    [Authorize(Policy = "CanDeleteProductCategory")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProductCategoryDelete(int id)
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
