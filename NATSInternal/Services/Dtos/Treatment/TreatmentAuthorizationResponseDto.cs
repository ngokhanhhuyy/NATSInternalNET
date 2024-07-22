namespace NATSInternal.Services.Dtos;

public class TreatmentAuthorizationResponseDto
{
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanSetOrderedDateTime { get; set; }
}