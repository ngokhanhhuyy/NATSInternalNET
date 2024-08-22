namespace NATSInternal.Services.Entities;

[Table("notifications")]
public class Notification
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("notification_type")]
    [Required]
    public NotificationType Type { get; set; }

    [Column("datetime")]
    [Required]
    public DateTime DateTime { get; set; }
    
    [Column("resource_ids")]
    public List<int> ResourceIds { get; set; }

    // Navigation properties.
    public virtual List<User> ReceivedUsers { get; set; }
    public virtual List<User> ReadUsers { get; set; }
    
    // Computed properies.
    [NotMapped]
    public int ResourcePrimaryId => ResourceIds[0];
    
    [NotMapped]
    public int ResourceSecondaryId => ResourceIds[1];
}
