namespace NATSInternal.Services.Entities;

public class LockableEntity : ILockableEntity
{
    private DateTime _statsDateTime;
    
    [Column("created_datetime")]
    [Required]
    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow.ToApplicationTime();
    
    [NotMapped]
    protected DateTime StatsDateTime
    {
        get => _statsDateTime;
        set
        {
            string errorMessage;
            if (_statsDateTime > CreatedDateTime)
            {
                errorMessage = ErrorMessages.EarlierThanOrEqual
                    .ReplaceComparisonValue(CreatedDateTime.ToVietnameseString());
                throw new ArgumentException(errorMessage);
            }
            
            DateTime minimumAssignableDateTime;
            minimumAssignableDateTime = new DateTime(
                CreatedDateTime.AddMonths(-1).Year,
                CreatedDateTime.AddMonths(-1).Month,
                1, 0, 0, 0);
            if (_statsDateTime < minimumAssignableDateTime)
            {
                errorMessage = ErrorMessages.GreaterThanOrEqual
                    .ReplaceComparisonValue(minimumAssignableDateTime.ToVietnameseString());
                throw new ArgumentException(errorMessage);
            }
            
            _statsDateTime = value;
        }
    }
    
    [NotMapped]
    protected DateTime LockedDateTime
    {
        get
        {
            DateOnly lockedMonthAndYear = DateOnly
                .FromDateTime(CreatedDateTime)
                .AddMonths(2);
            return new DateTime(
                lockedMonthAndYear.Year, lockedMonthAndYear.Month, 1,
                0, 0, 0);
        }
    }
    
    [NotMapped]
    public bool IsLocked
    {
        get
        {
            DateTime currentDateTime = DateTime.UtcNow.ToApplicationTime();
            return currentDateTime >= LockedDateTime;
        }
    }
}