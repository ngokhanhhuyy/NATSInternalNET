namespace NATSInternal.Controllers;

[Route("Api/Notification")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _service;
    private readonly IValidator<NotificationListRequestDto> _listValidator;
    
    public NotificationController(
            INotificationService service,
            IValidator<NotificationListRequestDto> listValidator)
    {
        _service = service;
        _listValidator = listValidator;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> NotificationList(
            [FromQuery] NotificationListRequestDto requestDto)
    {
        // Validate data from the request.
        ValidationResult validationResult;
        validationResult = _listValidator.Validate(requestDto.TransformValues());
        if (!validationResult.IsValid)
        {
            ModelState.AddModelErrorsFromValidationErrors(validationResult.Errors);
            return BadRequest(ModelState);
        }
        
        // Fetch the list.
        NotificationListResponseDto responseDto = await _service
            .GetListAsync(requestDto);
        
        // Generate the resource URL for the notifications.
        foreach (NotificationResponseDto notification in responseDto.Items)
        {
            notification.GenerateResourceUrl(Url);
        }
        
        return Ok(responseDto);
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> NotificationSingle(int id)
    {
        try
        {
            // Fetch the notification.
            NotificationResponseDto responseDto = await _service.GetSingleAsync(id);
            
            // Generate the resource URL for the notification.
            responseDto.GenerateResourceUrl(Url);
            
            return Ok(responseDto);
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpPost("{id:int}/MarkAsRead")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> NotificationMarkAsRead(int id)
    {
        try
        {
            await _service.MarkAsReadAsync(id);
            return Ok();
        }
        catch (ResourceNotFoundException)
        {
            return NotFound();
        }
    }
}