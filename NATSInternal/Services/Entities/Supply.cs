namespace NATSInternal.Services.Entities;

[Table("supplies")]
public class Supply
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("supplied_datetime")]
    [Required]
    public DateTime SuppliedDateTime { get; set; }

    [Column("shipment_fee")]
    [Required]
    public long ShipmentFee { get; set; } = 0;

    [Column("note")]
    [StringLength(255)]
    public string Note { get; set; }

    [Column("created_datetime")]
    [Required]
    public DateTime CreatedDateTime { get; set; }

    [Column("is_closed")]
    [Required]
    public bool IsClosed { get; set; }

    // Foreign keys
    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    // Concurrency operation tracking field
    [Column("row_version")]
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Relationships
    public virtual User User { get; set; }
    public virtual List<SupplyItem> Items { get; set; }
    public virtual List<SupplyPhoto> Photos { get; set; }
    public virtual List<SupplyUpdateHistories> UpdateHistories { get; set; }

    // Properties for convinience.
    [NotMapped]
    public long ItemAmount => Items.Sum(i => i.Amount);

    [NotMapped]
    public long TotalAmount => ItemAmount + ShipmentFee;

    [NotMapped]
    public DateTime? UpdatedDateTime => UpdateHistories
        .OrderByDescending(uh => uh.UpdatedDateTime)
        .Select(uh => uh.UpdatedDateTime)
        .Cast<DateTime?>()
        .FirstOrDefault();
}