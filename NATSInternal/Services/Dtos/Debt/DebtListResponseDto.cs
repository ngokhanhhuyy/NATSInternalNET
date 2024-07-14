namespace NATSInternal.Services.Dtos;

public class DebtListResponseDto
{
    public int PageCount { get; set; }
    public List<DebtBasicResponseDto> Items { get; set; }
    public DebtListAuthorizationResponseDto Authorization { get; set; }
}
