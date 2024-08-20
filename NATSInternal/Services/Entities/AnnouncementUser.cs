namespace NATSInternal.Services.Entities;

[Table("announcement_read_users")]
public class AnnouncementReadUser
{
    // Primary key
    [Column("announcement_id")]
    public int AnnouncementId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    // Navigation properties.
    public virtual Announcement Announcement { get; set; }
    public virtual User ReadUser { get; set; }
}