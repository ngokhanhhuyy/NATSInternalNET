namespace NATSInternal.Services.Dtos;

public class TreatmentTherapistUpdateHistoryDataDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }

    public TreatmentTherapistUpdateHistoryDataDto(User user)
    {
        Id = user.Id;
        UserName = user.UserName;
        FirstName = user.FirstName;
        MiddleName = user.MiddleName;
        LastName = user.LastName;
        FullName = user.FullName;
    }
}