namespace NATSInternal.Services.Dtos;

public class AnnouncementListRequestDto : IRequestDto<AnnouncementListRequestDto>
{
    public bool NewAnnouncementsOnly { get; set; } = true;
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;

    public AnnouncementListRequestDto TransformValues()
    {
        return this;
    }
}
