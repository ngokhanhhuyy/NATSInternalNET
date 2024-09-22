using FluentValidation;

namespace NATSInternal.Services.Validations.Rules;

public static class RuleValidators
{
    public static IRuleBuilderOptions<T, DateTime?> LaterThanDateTime<T>(
            this IRuleBuilder<T, DateTime?> ruleBuilder,
            DateTime comparisonDateTime)
    {
        string errorMessage = ErrorMessages.LaterThan
            .ReplaceComparisonValue(comparisonDateTime.ToVietnameseString());
	    return ruleBuilder.GreaterThan(comparisonDateTime).WithMessage(errorMessage);
    }
    
    public static IRuleBuilderOptions<T, DateOnly> LaterThanDate<T>(
            this IRuleBuilder<T, DateOnly> ruleBuilder,
            DateOnly comparisonDate)
    {
        string errorMessage = ErrorMessages.GreaterThan
            .ReplaceComparisonValue(comparisonDate.ToVietnameseString());
        return ruleBuilder.GreaterThan(comparisonDate).WithMessage(errorMessage);
    }
    
    public static IRuleBuilderOptions<T, DateTime?> LaterThanOrEqualToDateTime<T>(
            this IRuleBuilder<T, DateTime?> ruleBuilder,
            DateTime comparisonDateTime)
    {
        string errorMessage = ErrorMessages.GreaterThanOrEqual
            .ReplaceComparisonValue(comparisonDateTime.ToVietnameseString());
        return ruleBuilder.GreaterThanOrEqualTo(comparisonDateTime).WithMessage(errorMessage);
    }
    
    public static IRuleBuilderOptions<T, DateOnly?> LaterThanOrEqualToDate<T>(
            this IRuleBuilder<T, DateOnly?> ruleBuilder,
            DateOnly comparisonDate)
    {
        string errorMessage = ErrorMessages.GreaterThanOrEqual
            .ReplaceComparisonValue(comparisonDate.ToVietnameseString());
        return ruleBuilder.GreaterThanOrEqualTo(comparisonDate).WithMessage(errorMessage);
    }
    
    public static IRuleBuilderOptions<T, DateTime?> EarlierThanOrEqualToNow<T>(
            this IRuleBuilder<T, DateTime?> ruleBuilder)
    {
        DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
        string errorMessage = ErrorMessages.EarlierThanOrEqualToNow
            .ReplaceComparisonValue(currentDateTime.ToVietnameseString());
        return ruleBuilder.LessThanOrEqualTo(currentDateTime).WithMessage(errorMessage);
    }

    public static IRuleBuilderOptions<T, byte[]> IsValidImage<T>(
            this IRuleBuilder<T, byte[]> ruleBuilder)
    {
        return ruleBuilder.Must(file =>
        {
            try
            {
                MagickImage image = new MagickImage(file);
                return true;
            }
            catch (MagickMissingDelegateErrorException)
            {
                return false;
            }
        }).WithMessage(ErrorMessages.Invalid);
    }
    
    public static IRuleBuilderOptions<T, DateTime?> IsValidStatsDateTime<T>(
            this IRuleBuilder<T, DateTime?> ruleBuilder)
    {
        return ruleBuilder.Must(dateTime =>
        {
            if (dateTime != null)
            {
                DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
                if (dateTime > currentDateTime)
                {
                    return false;
                }
            }
            return true;
        }).WithMessage(_ =>
        {
            DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
            return ErrorMessages.EarlierThanOrEqualToNow
                .ReplaceComparisonValue(currentDateTime.ToVietnameseString());
        })
        .Must(dateTime =>
        {
            if (dateTime != null)
            {
                if (dateTime < MinimumStatsDateTime)
                {
                    return false;
                }
            }
            
            return true;
        }).WithMessage(ErrorMessages.LaterThanOrEqual.ReplaceComparisonValue(
            MinimumStatsDateTime.ToVietnameseString()));
    }

    public static IRuleBuilderOptions<T, int> IsValidQueryStatsYear<T>(
            this IRuleBuilder<T, int> ruleBuilder) where T : ILockableEntityListRequestDto
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(DateTime.UtcNow.ToApplicationTime().Year);
    }

    public static IRuleBuilderOptions<T, int> IsValidQueryStatsMonth<T>(
            this IRuleBuilder<T, int> ruleBuilder) where T : ILockableEntityListRequestDto
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(DateTime.UtcNow.ToApplicationTime().Month)
            .When(dto => dto.Year == DateTime.UtcNow.ToApplicationTime().Year)
            .LessThanOrEqualTo(12)
            .When(dto => dto.Year < DateTime.UtcNow.ToApplicationTime().Year);
    }
    
    private static DateTime MinimumStatsDateTime
    {
        get
        {
            DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
            return new DateTime(
                currentDateTime.AddMonths(-1).Year,
                currentDateTime.AddMonths(-1).Month,
                1, 0, 0, 0);
        }
    }
}