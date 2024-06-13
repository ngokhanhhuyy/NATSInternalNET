namespace NATSInternal.Services.Utilities;

public class IdentifierCasingUtility
{
    public static string SnakeCaseToPascalCase(string snakeCaseString)
    {
        string[] snakeCaseSegments = snakeCaseString.Split("_");
        List<string> pascalCaseSegments = new List<string>();
        foreach (string snakeCaseSegment in snakeCaseSegments)
        {
            if (snakeCaseSegment.Length == 0)
            {
                pascalCaseSegments.Add(string.Empty);
                continue;
            }
            string pascalCaseSegment = snakeCaseSegment.First().ToString().ToUpper();
            if (snakeCaseSegment.Length > 1)
            {
                pascalCaseSegment += snakeCaseSegment[1..];
            }
            pascalCaseSegments.Add(pascalCaseSegment);
        }
        return string.Join("", pascalCaseSegments);
    }

    public static string SnakeCaseToCamelCase(string snakeCaseString)
    {
        string[] snakeCaseSegments = snakeCaseString.Split("_");
        List<string> pascalCaseSegments = new List<string>();
        foreach (string snakeCaseSegment in snakeCaseSegments)
        {
            if (snakeCaseSegment.Length == 0)
            {
                pascalCaseSegments.Add(string.Empty);
                continue;
            }
            string pascalCaseSegment = snakeCaseSegment.First().ToString().ToUpper();
            if (snakeCaseSegment.Length > 1)
            {
                pascalCaseSegment += snakeCaseSegment[1..];
            }
            pascalCaseSegments.Add(pascalCaseSegment);
        }
        return string.Join("", pascalCaseSegments);
    }
}
