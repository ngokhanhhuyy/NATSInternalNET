namespace NATSInternal.Services.Entities;

[Table("announcements")]
public class Announcement
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("category")]
    [Required]
    public AnnouncementCategory Category { get; set; } = AnnouncementCategory.Announcement;

    [Column("title")]
    [Required]
    [StringLength(80)]
    public string Title { get; set; }

    [Column("content")]
    [Required]
    [StringLength(5000)]
    public string Content { get; set; }

    [Column("starting_datetime")]
    [Required]
    public DateTime StartingDateTime { get; set; }

    [Column("ending_datetime")]
    [Required]
    public DateTime EndingDateTime { get; set; }

    // Foreign keys
    [Column("created_user_id")]
    public int CreatedUserId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Navigation properties
    public virtual User CreatedUser { get; set; }
    public virtual List<User> ReadUsers { get; set; }
}