namespace NATSInternal.Services.Entities;

[Table("order_photos")]
public class OrderPhoto
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("url")]
    [Required]
    [StringLength(255)]
    public string Url { get; set; }

    // Foreign keys
    [Column("order_id")]
    [Required]
    public int OrderId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Relationship
    public virtual Order Order { get; set; }
}