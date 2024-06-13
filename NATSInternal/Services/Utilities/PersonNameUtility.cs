namespace NATSInternal.Services.Utilities;

public class PersonNameUtility
{
    public static string GetFullNameFromNameElements(string firstName, string middleName, string lastName)
    {
        IEnumerable<string> nameElements = new[] { firstName, middleName, lastName }
            .Where(name => !string.IsNullOrEmpty(name));
        return string.Join(" ", nameElements);
    }

    public static string GetFullNameFromNameElements(PersonNameElementsDto nameElementsDto)
    {
        IEnumerable<string> nameElements = new[] {
                nameElementsDto.FirstName,
                nameElementsDto.MiddleName,
                nameElementsDto.LastName
            }
            .Where(name => !string.IsNullOrEmpty(name));
        return string.Join(" ", nameElements);
    }

    public static PersonNameElementsDto GetNameElementsFromFullName(string fullName)
    {
        List<string> nameElements = fullName.Split(" ").ToList();
        string middleName = string.Join(" ", nameElements.Skip(1).SkipLast(1));
        bool isMiddleNameEmpty = string.IsNullOrWhiteSpace(middleName);
        return new PersonNameElementsDto
        {
            FirstName = nameElements[0],
            LastName = nameElements.Last(),
            MiddleName = isMiddleNameEmpty ? null : middleName
        };
    }
}
