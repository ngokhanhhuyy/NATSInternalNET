namespace NATSInternal.Services.Dtos;

public class UserListRequestDto : IRequestDto<UserListRequestDto>
{
    public string OrderByField { get; set; } = nameof(FieldToBeOrdered.LastName);
    public bool OrderByAscending { get; set; } = true;
    public int? RoleId { get; set; }
    public string Content { get; set; }
    public int Page { get; set; } = 1;
    public int ResultsPerPage { get; set; } = 15;

    public UserListRequestDto TransformValues()
    {
        OrderByField = OrderByField?.ToNullIfEmpty()?.CamelCaseToPascalCase();
        Content = Content?.ToNullIfEmpty();
        return this;
    }

    public enum FieldToBeOrdered
    {
        LastName,
        FirstName,
        UserName,
        Birthday,
        CreatedDateTime,
        Role,
    }
}