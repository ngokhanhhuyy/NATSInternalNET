namespace NATSInternal.Services.Entities;

[Table("supply_photos")]
public class SupplyPhoto
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("url")]
    [Required]
    [StringLength(255)]
    public string Url { get; set; }

    // Foreign keys
    [Column("supply_id")]
    [Required]
    public int SupplyId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Navigation properties
    public virtual Supply Supply { get; set; }
}