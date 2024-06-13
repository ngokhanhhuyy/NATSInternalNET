namespace NATSInternal.Services.Dtos;

public class IntroducerSearchRequestDto : IRequestDto<IntroducerSearchRequestDto> {
    public string SearchByContent { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;

    public IntroducerSearchRequestDto TransformValues() {
        SearchByContent = SearchByContent?.ToNullIfEmpty();
        return this;
    }
}