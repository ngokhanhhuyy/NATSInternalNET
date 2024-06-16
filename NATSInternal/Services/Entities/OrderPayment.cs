namespace NATSInternal.Services.Entities;

[Table("order_payments")]
public class OrderPayment
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("amount")]
    [Required]
    public long Amount { get; set; }

    [Column("paid_datetime")]
    [Required]
    public DateTime PaidDateTime { get; set; }

    [Column("note")]
    [Required]
    [StringLength(255)]
    public string Note { get; set; }

    [Column("is_closed")]
    [Required]
    public bool IsClosed { get; set; } = false;

    // Foreign keys
    [Column("order_id")]
    [Required]
    public int OrderId { get; set; }

    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Relationships
    public virtual Order Order { get; set; }
    public virtual User User { get; set; }
}