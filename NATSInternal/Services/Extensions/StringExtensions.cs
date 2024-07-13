using System.Globalization;
using System.Text;

namespace NATSInternal.Services.Extensions;

public static class StringExtensions
{
    public static string ToNullIfEmpty(this string value)
    {
        return string.IsNullOrWhiteSpace(value?.Trim()) ? null : value.Trim();
    }

    public static string ToWordsFirstLetterCapitalized(this string value)
    {
        string result = string.Empty;
        for (int i = 0; i < value.Length; i++) {
            if (i == 0 || value[i - 1] == ' ' || value[i - 1] == '\n') {
                result += value[i].ToString().ToUpper();
                continue;
            }
            result += value[i];
        }
        return result;
    }

    public static string CamelCaseToPascalCase(this string camelCaseString)
    {
        string[] camelCaseWords = camelCaseString.Split("_");
        List<string> pascalCaseWords = new List<string>();
        foreach (string camelCaseWord in camelCaseWords)
        {
            string pascalCaseWord = camelCaseWord[0].ToString().ToUpper();
            if (camelCaseWord.Length > 1)
            {
                pascalCaseWord += camelCaseWord[1..];
            }
            pascalCaseWords.Add(pascalCaseWord);
        }
        return string.Join("", pascalCaseWords);
    }
    
    public static string ToNonDiacritics(this string value)
    {
        string normalizedString = value.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new StringBuilder();
    
        foreach (Rune rune in normalizedString.EnumerateRunes())
        {
            var unicodeCategory = Rune.GetUnicodeCategory(rune);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(rune);
            }
        }
    
        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .Replace('đ', 'd')
            .Replace('Đ', 'D')
            .Replace('Ð', 'D');
    }

    public static string ReplaceResourceName(this string value, string resouceDisplayName)
    {
        return value.Replace("{ResourceName}", resouceDisplayName);
    }

    public static string ReplacePropertyName(this string value, string propertyDisplayName)
    {
        return value.Replace("{PropertyName}", propertyDisplayName);
    }

    public static string ReplaceAttemptedValue(this string value, string attemptedValue)
    {
        return value.Replace("{AttemptedValue}", attemptedValue);
    }

    public static string ReplaceComparisonValue(this string value, string comparisonValue)
    {
        return value.Replace("{ComparisonValue}", comparisonValue);
    }
}