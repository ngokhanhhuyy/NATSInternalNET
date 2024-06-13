using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.RegularExpressions;

namespace NATSInternal.Services.Results;

public sealed partial class ServiceError
{
    public ServiceErrorType ErrorType { get; init; }
    public string PropertyName { get; init; }
    public string ErrorMessage { get; init; }

    public static ServiceError FromIdentityError(IdentityError error, string propertyName = null)
    {
        return new ServiceError
        {
            PropertyName = propertyName,
            ErrorMessage = error.Description
        };
    }

    public static ServiceError Undefined()
    {
        return new ServiceError
        {
            ErrorType = ServiceErrorType.UndefinedError,
            ErrorMessage = ErrorMessages.Undefined
        };
    }

    public static ServiceError Mismatched(string propertyName)
    {
        string key = GetKeyForDisplayName(propertyName);
        return new ServiceError
        {
            ErrorType = ServiceErrorType.ValidationError,
            ErrorMessage = ErrorMessages.MismatchedWith
                .Replace("{ComparisonPropertyName}", DisplayNames.Get(key))
        };
    }

    public static ServiceError Invalid(string propertyName)
    {
        string key = GetKeyForDisplayName(propertyName);
        return new ServiceError
        {
            ErrorType = ServiceErrorType.ValidationError,
            PropertyName = propertyName,
            ErrorMessage = ErrorMessages.Invalid
                .Replace("{PropertyName}", DisplayNames.Get(key))
        };
    }

    public static ServiceError Incorrect(string propertyName)
    {
        string key = GetKeyForDisplayName(propertyName);
        return new ServiceError
        {
            ErrorType = ServiceErrorType.OperationError,
            PropertyName = propertyName,
            ErrorMessage = ErrorMessages.Incorrect
                .Replace("{PropertyName}", DisplayNames.Get(key))
        };
    }

    public static ServiceError UniqueDuplicated(string propertyName)
    {
        string key = GetKeyForDisplayName(propertyName);
        return new ServiceError
        {
            ErrorType = ServiceErrorType.OperationError,
            PropertyName = propertyName,
            ErrorMessage = ErrorMessages.UniqueDuplicated
                .Replace("{PropertyName}", DisplayNames.Get(key))
        };
    }

    public static ServiceError NotFound(string entityName)
    {
        return new ServiceError
        {
            ErrorType = ServiceErrorType.NotFoundError,
            ErrorMessage = ErrorMessages.NotFound
                .Replace("{EntityName}", DisplayNames.Get(entityName))
        };
    }

    public static ServiceError NotFoundByProperty(string entityName, string propertyName, string propertyValue)
    {
        string key = GetKeyForDisplayName(propertyName);
        return new ServiceError
        {
            ErrorType = ServiceErrorType.NotFoundError,
            PropertyName = propertyName,
            ErrorMessage = ErrorMessages.NotFoundByProperty
                .Replace("{EntityName}", DisplayNames.Get(entityName))
                .Replace("{PropertyName}", DisplayNames.Get(key))
                .Replace("{PropertyValue}", propertyValue)
        };
    }

    public static ServiceError GreaterThanOrEqual(string propertyName, object comparisonValue)
    {
        string key = GetKeyForDisplayName(propertyName);
        return new ServiceError
        {
            ErrorType = ServiceErrorType.ValidationError,
            PropertyName = propertyName,
            ErrorMessage = ErrorMessages.GreaterThanOrEqual
                .Replace("{PropertyName}", DisplayNames.Get(key))
                .Replace("{ComparisonValue}", comparisonValue.ToString())
        };
    }

    public static ServiceError NotAvailable(string entityName)
    {
        string key = GetKeyForDisplayName(entityName);
        return new ServiceError
        {
            ErrorType = ServiceErrorType.OperationError,
            ErrorMessage = ErrorMessages.NotAvailable
                .Replace("{EntityName}", DisplayNames.Get(key))
        };
    }

    public static ServiceError NotAvailableByProperty(
            string entityName,
            string propertyName,
            string propertyValue)
    {
        return new ServiceError
        {
            ErrorType = ServiceErrorType.OperationError,
            ErrorMessage = ErrorMessages.NotAvailableByProperty
                .Replace("{EntityName}", DisplayNames.Get(entityName))
                .Replace("{PropertyName}", DisplayNames.Get(propertyName))
                .Replace("{PropertyValue}", propertyValue)
        };
    }

    private static string GetKeyForDisplayName(string propertyName)
    {
        // Validating value and extracting the last element of the value as key for display name
        if (propertyName != null)
        {
            ArgumentException exception = new ArgumentException($"{propertyName} is not a valid PropertyName");
            // Checking if value has valid pattern
            if (!PropertyNameRegexPattern().IsMatch(propertyName))
            {
                throw exception;
            }
            string[] elements = propertyName.Split(".");
            List<object> keys = new List<object>();
            foreach (string element in elements)
            {
                // Validating index part of current element if containing brackets
                if (element.Contains('['))
                {
                    bool hasOneOpeningBracket = element.Count(c => c == '[') == 1;
                    bool hasOneClosingBracket = element.Count(c => c == ']') == 1;
                    if (!hasOneOpeningBracket || !hasOneClosingBracket || !element.EndsWith("]"))
                    {
                        throw exception;
                    }
                    string[] elementWithIndex = PropertyNameElementWithIndex().Split(element);
                    keys.Add(elementWithIndex[0]);
                    // Validating if index is valid int
                    if (int.TryParse(elementWithIndex[1], out int index))
                    {
                        keys.Add(index);
                    }
                    else
                    {
                        throw exception;
                    }
                }
                else
                {
                    keys.Add(element);
                }
            };
            return keys.ToArray()
                .Reverse()
                .Where(name => name is string && name != null)
                .Select(name => name.ToString().ToWordsFirstLetterCapitalized())
                .First();
        }
        return propertyName;
    }

    [GeneratedRegex("[\\.A-Za-z0-9\\[\\]]")]
    private static partial Regex PropertyNameRegexPattern();
    [GeneratedRegex("\\[|\\]")]
    private static partial Regex PropertyNameElementWithIndex();
}