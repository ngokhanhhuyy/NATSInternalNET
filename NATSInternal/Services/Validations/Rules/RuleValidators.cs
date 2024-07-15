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
}