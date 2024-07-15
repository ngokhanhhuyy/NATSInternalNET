namespace NATSInternal.Services.Dtos;

public class ConsultantAuthorizationResponseDto
{
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanSetPaidDateTime { get; set; }
}