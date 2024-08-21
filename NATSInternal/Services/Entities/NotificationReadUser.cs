namespace NATSInternal.Services.Entities;

[Table("notification_read_users")]
public class NotificationReadUser
{
    [Column("read_notification_id")]
    [Key]
    public int ReadNotificationId { get; set; }

    [Column("read_user_id")]
    [Key]
    public int ReadUserId { get; set; }

    // Navigation properties.
    public virtual Notification ReadNotification { get; set; }
    public virtual User ReadUser { get; set; }
}
