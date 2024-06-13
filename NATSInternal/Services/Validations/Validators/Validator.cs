using System.Text.RegularExpressions;

namespace NATSInternal.Services.Validations.Validators;

public partial class Validator<TRequestDto> : AbstractValidator<TRequestDto>
        where TRequestDto : IRequestDto<TRequestDto> {
    public Validator() {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;
    }

    protected string PhoneNumberRegex = @"^[0-9]*$";
    protected string EmailRegex = @"^\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b$";

    protected virtual bool EqualOrEarlierThanToday(DateTime value) {
        return value <= DateTime.UtcNow.Date;
    }

    protected virtual bool EqualOrEarlierThanToday(DateTime? value) {
        if (value.HasValue) {
            return EqualOrEarlierThanToday(value.Value);
        }
        return true;
    }

    protected virtual bool EqualOrEarlierThanToday(DateOnly value)
    {
        return value.ToDateTime(new TimeOnly(0, 0)) <= DateTime.UtcNow.Date;
    }

    protected virtual bool EqualOrEarlierThanToday(DateOnly? value)
    {
        if (value.HasValue)
        {
            return EqualOrEarlierThanToday(value.Value);
        }
        return true;
    }

    protected virtual bool ValidIdCardNumber(string value) {
        if (value != null) {
            Regex regex = new Regex(@"[0-9]");
            return regex.Matches(value).Any();
        }
        return true;
    }

    protected virtual bool IsValidImage(byte[] imageAsBytes)
    {
        try
        {
            MagickImage image = new MagickImage(imageAsBytes);
            return true;
        }
        catch (MagickMissingDelegateErrorException)
        {
            return false;
        }
    }

    protected virtual bool IsEnumElementName<TEnum>(string name) where TEnum : Enum {
        return name != null && (
            Enum.GetNames(typeof(TEnum)).ToList().Contains(name) ||
            Enum.GetNames(typeof(TEnum)).ToList().Contains(name.CamelCaseToPascalCase()));
    }
}