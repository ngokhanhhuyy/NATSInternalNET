namespace NATSInternal.Services.Entities;

[Table("consultant")]
public class Consultant
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("amount")]
    [Required]
    public long Amount { get; set; }

    [Column("note")]
    [StringLength(255)]
    public string Note { get; set; }

    [Column("paid_datetime")]
    [Required]
    public DateTime PaidDateTime { get; set; }

    [Column("is_closed")]
    [Required]
    public bool IsClosed { get; set; }

    [Column("is_deleted")]
    [Required]
    public bool IsDeleted { get; set; }

    // Foreign keys.
    [Column("customer_id")]
    [Required]
    public int CustomerId { get; set; }

    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    // Navigation properties.
    [NotMapped]
    public virtual Customer Customer { get; set; }

    [NotMapped]
    public virtual User User { get; set; }
}