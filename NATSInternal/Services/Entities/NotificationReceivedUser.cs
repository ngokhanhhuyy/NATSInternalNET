namespace NATSInternal.Services.Entities;

[Table("notification_received_users")]
public class NotificationReceivedUser
{
    [Column("received_notification_id")]
    [Key]
    public int ReceivedNotificationId { get; set; }

    [Column("received_user_id")]
    [Key]
    public int ReceivedUserId { get; set; }

    // Navigation properties.
    public virtual Notification ReceivedNotification { get; set; }
    public virtual User ReceivedUser { get; set; }
}
