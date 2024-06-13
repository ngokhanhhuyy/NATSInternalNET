namespace NATSInternal.Services.Entities;

[Table("order_items")]
public class OrderItem
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("amount")]
    [Required]
    public long Amount { get; set; }

    [Column("vat_factor")]
    [Required]
    public decimal VatFactor { get; set; } = 0.1M;

    [Column("quantity")]
    [Required]
    public int Quantity { get; set; } = 1;

    // Foreign keys
    [Column("order_id")]
    [Required]
    public int OrderId { get; set; }

    [Column("product_id")]
    [Required]
    public int ProductId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Relationships
    public virtual Order Order { get; set; }
    public virtual Product Product { get; set; }
}