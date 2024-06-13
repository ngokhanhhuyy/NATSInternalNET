namespace NATSInternal.Models;

public class UserListModel
{
    public string OrderByField { get; set; }
    public bool OrderByAscending { get; set; } = true;
    public RoleBasicModel Role { get; set; }
    public string Content { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;
    public int PageCount { get; set; }
    public List<UserBasicModel> Results { get; set; }
    public List<RoleBasicModel> RoleOptions { get; set; }

    public void MapFromResponseDtos(
            UserListResponseDto responseDto,
            RoleListResponseDto roleListResponseDto)
    {
        PageCount = responseDto.PageCount;
        Results = responseDto.Results
            .Select(UserBasicModel.FromResponseDto)
            .ToList();
        RoleOptions = roleListResponseDto.Items
            .Select(RoleBasicModel.FromResponseDto)
            .ToList();
        Role ??= RoleOptions.OrderBy(r => r.PowerLevel).First();
    }

    public UserListRequestDto ToRequestDto()
    {
        return new UserListRequestDto
        {
            OrderByField = OrderByField ?? nameof(UserListRequestDto.FieldToBeOrdered.LastName),
            OrderByAscending = OrderByAscending,
            RoleId = Role?.Id,
            Content = Content,
            Page = Page,
        };
    }
}