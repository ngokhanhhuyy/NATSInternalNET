namespace NATSInternal.Services.Entities;

public class LockableEntity : ILockableEntity
{
    
    [Column("created_datetime")]
    [Required]
    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow.ToApplicationTime();
    
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