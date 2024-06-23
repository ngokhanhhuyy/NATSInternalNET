namespace NATSInternal.Services.Entities;

public class UserRefreshToken
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("token")]
    [Required]
    [StringLength(2048)]
    public string Token { get; set; }

    [Column("issued_datetime")]
    [Required]
    public DateTime IssuedDateTime { get; set; } = DateTime.UtcNow.ToApplicationTime();

    [Column("expiring_datetime")]
    [Required]
    public DateTime ExpiringDateTime { get; set; }

    // Foreign keys.
    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    // Navigation properties.
    public virtual User User { get; set; }
}
