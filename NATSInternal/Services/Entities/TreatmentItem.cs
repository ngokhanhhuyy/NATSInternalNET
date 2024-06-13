namespace NATSInternal.Services.Entities;

[Table("treatment_items")]
public class TreatmentItem
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
    public int Quantity { get; set; }

    // Foreign keys
    [Column("session_id")]
    [Required]
    public int SessionId { get; set; }

    [Column("product_id")]
    [Required]
    public int ProductId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Relationship
    public virtual TreatmentSession Session { get; set; }
    public virtual Product Product { get; set; }
}