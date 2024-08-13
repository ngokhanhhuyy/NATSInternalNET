namespace NATSInternal.Services.Entities;

[Table("supplies")]
public class Supply : LockableEntity
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("paid_datetime")]
    [Required]
    public DateTime PaidDateTime { get; set; }

    [Column("shipment_fee")]
    [Required]
    public long ShipmentFee { get; set; } = 0;

    [Column("note")]
    [StringLength(255)]
    public string Note { get; set; }

    [Column("is_deleted")]
    [Required]
    public bool IsDeleted { get; set; }

    // Foreign keys
    [Column("created_user_id")]
    [Required]
    public int CreatedUserId { get; set; }

    // Concurrency operation tracking field
    [Column("row_version")]
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Relationships
    public virtual User CreatedUser { get; set; }
    public virtual List<SupplyItem> Items { get; set; }
    public virtual List<SupplyPhoto> Photos { get; set; }
    public virtual List<SupplyUpdateHistory> UpdateHistories { get; set; }

    // Properties for convinience.
    [NotMapped]
    public long ItemAmount => Items.Sum(i => i.Amount * i.SuppliedQuantity);

    [NotMapped]
    public long TotalAmount => ItemAmount + ShipmentFee;

    [NotMapped]
    public DateTime? UpdatedDateTime => UpdateHistories
        .OrderByDescending(uh => uh.UpdatedDateTime)
        .Select(uh => uh.UpdatedDateTime)
        .Cast<DateTime?>()
        .FirstOrDefault();

    [NotMapped]
    public string FirstPhotoUrl => Photos
        .OrderBy(p => p.Id)
        .Select(p => p.Url)
        .FirstOrDefault();

    [NotMapped]
    public DateTime? LastUpdatedDateTime => UpdateHistories
        .OrderBy(uh => uh.UpdatedDateTime)
        .Select(uh => (DateTime?)uh.UpdatedDateTime)
        .LastOrDefault();

    [NotMapped]
    public User LastUpdatedUser => UpdateHistories
        .OrderBy(uh => uh.UpdatedDateTime)
        .Select(uh => uh.User)
        .LastOrDefault();
}