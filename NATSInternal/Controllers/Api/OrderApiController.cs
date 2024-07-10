namespace NATSInternal.Controllers.Api;

[Route("Api/Order")]
[ApiController]
[Authorize]
public class OrderApiController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IOrderPaymentService _orderPaymentService;
    private readonly IValidator<OrderListRequestDto> _listValidator;
    private readonly IValidator<OrderUpsertRequestDto> _upsertValidator;
    private readonly IValidator<OrderPaymentRequestDto> _paymentValidator;

    public OrderApiController(
            IOrderService orderService,
            IOrderPaymentService orderPaymentService,
            IValidator<OrderListRequestDto> listValidator,
            IValidator<OrderUpsertRequestDto> upsertValidator,
            IValidator<OrderPaymentRequestDto> paymentValidator)
    {
        _orderService = orderService;
        _orderPaymentService = orderPaymentService;
        _listValidator = listValidator;
        _upsertValidator = upsertValidator;
        _paymentValidator = paymentValidator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetListAsync([FromQuery] OrderListRequestDto requestDto)
    {
        ValidationResult validationResult;
        validationResult = _listValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        return Ok(await _orderService.GetListAsync(requestDto));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id)
    {
        try
        {
            return Ok(await _orderService.GetDetailAsync(id));
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }

    [HttpPost]
    [Authorize(Policy = "CanCreateOrder")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateAsync([FromBody] OrderUpsertRequestDto requestDto)
    {
        ValidationResult validationResult;
        validationResult = _upsertValidator.Validate(
            requestDto.TransformValues(),
            options => options.IncludeRuleSets("Create").IncludeRulesNotInRuleSet());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        try
        {
            int createdId = await _orderService.CreateAsync(requestDto);
            string createdUrl = Url.Action("OrderDetail", "OrderApi", new { id = createdId });
            return Created(createdUrl, createdId);
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(exception);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "CanEditOrder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateAsync(
            int id,
            [FromBody] OrderUpsertRequestDto requestDto)
    {
        ValidationResult validationResult;
        validationResult = _upsertValidator.Validate(
            requestDto.TransformValues(),
            options => options.IncludeRuleSets("Update").IncludeRulesNotInRuleSet());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        try
        {
            await _orderService.UpdateAsync(id, requestDto);
            return Ok();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
        catch (AuthorizationException)
        {
            return Forbid();
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(ModelState);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CanDeleteOrder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            await _orderService.DeleteAsync(id);
            return Ok();
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
        catch (AuthorizationException)
        {
            return Forbid();
        }
        catch (OperationException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return UnprocessableEntity(ModelState);
        }
    }

    [HttpGet("{id:int}/Payment/{orderId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentDetailAsync(int orderId)
    {
        try
        {
            return Ok(await _orderPaymentService.GetDetailAsync(orderId));
        }
        catch (ResourceNotFoundException exception)
        {
            ModelState.AddModelErrorsFromServiceException(exception);
            return NotFound(ModelState);
        }
    }

    [HttpPost("{id:int}/Payment")]
    [Authorize(Policy = "CanCreateOrderPayment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> OrderPaymentCreate(
            int id,
            [FromBody] OrderPaymentRequestDto requestDto)
    {
        ValidationResult validationResult = _paymentValidator.Validate(
            requestDto.TransformValues(),
            options =>
            {
                options.IncludeRuleSets("Create").IncludeRulesNotInRuleSet();
            });
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        try
        {
            OrderPaymentCreateResponseDto responseDto;
            responseDto = await _orderPaymentService.CreateAsync(id, requestDto);
            return Ok(responseDto);
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

    [HttpPut("{id:int}/Payment/{paymentId:int}")]
    [Authorize(Policy = "CanEditOrderPayment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> OrderPaymentUpdate(
            int paymentId,
            [FromBody] OrderPaymentRequestDto requestDto)
    {
        ValidationResult validationResult = _paymentValidator.Validate(
            requestDto,
            options =>
            {
                options.IncludeRuleSets("Update").IncludeRulesNotInRuleSet();
            });
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }

        try
        {
            await _orderPaymentService.UpdateAsync(paymentId, requestDto);
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

    [HttpDelete("{id:int}/Payment/{paymentId:int}")]
    [Authorize(Policy = "CanDeleteOrderPayment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> OrderPaymentDelete(int paymentId)
    {
        try
        {
            await _orderPaymentService.DeleteAsync(paymentId);
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
}
