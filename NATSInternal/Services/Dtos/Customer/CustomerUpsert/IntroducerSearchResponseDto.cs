namespace NATSInternal.Services.Dtos;

public class IntroducerSearchResponseDto {
    public int PageCount { get; init; }
    public int ResultsCount { get; init; }
    public List<CustomerBasicResponseDto> Results { get; init; }
}