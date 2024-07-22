namespace NATSInternal.Services.Dtos;

public class TreatmentListResponseDto
{
    public int PageCount { get; set; }
    public List<TreatmentBasicResponseDto> Items { get; set; }
    public TreatmentListAuthorizationResponseDto Authorization { get; set; }
}