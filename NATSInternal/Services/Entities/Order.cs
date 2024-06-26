namespace NATSInternal.Services.Entities;

[Table("orders")]
public class Order
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("ordered_datetime")]
    [Required]
    public DateTime OrderedDateTime { get; set; } = DateTime.UtcNow.ToApplicationTime();

    [Column("amount")]
    [Required]
    public long Amount { get; set; }

    [Column("note")]
    [StringLength(255)]
    public string Note { get; set; }

    [Column("is_closed")]
    [Required]
    public bool IsClosed { get; set; } = false;

    [Column("is_deleted")]
    [Required]
    public bool IsDeleted { get; set; } = false;

    // Foreign keys
    [Column("customer_id")]
    [Required]
    public int CustomerId { get; set; }

    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Relationships
    public virtual User User { get; set; }
    public virtual Customer Customer { get; set; }
    public virtual List<OrderItem> Items { get; set; }
    public virtual List<OrderPayment> Payments { get; set; }
    public virtual List<OrderPhoto> Photos { get; set; }
}