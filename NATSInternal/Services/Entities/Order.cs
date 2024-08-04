namespace NATSInternal.Services.Entities;

[Table("orders")]
public class Order : LockableEntity
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("paid_datetime")]
    [Required]
    public DateTime PaidDateTime { get; set; }

    [Column("note")]
    [StringLength(255)]
    public string Note { get; set; }

    [Column("is_deleted")]
    [Required]
    public bool IsDeleted { get; set; }

    // Foreign keys
    [Column("customer_id")]
    [Required]
    public int CustomerId { get; set; }

    [Column("created_user_id")]
    [Required]
    public int CreatedUserId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Relationships
    public virtual User CreatedUser { get; set; }
    public virtual Customer Customer { get; set; }
    public virtual List<OrderItem> Items { get; set; }
    public virtual List<OrderPhoto> Photos { get; set; }
    public virtual List<OrderUpdateHistory> UpdateHistories { get; set; }
    
    // Property for convinience.
    [NotMapped]
    public long BeforeVatAmount => Items.Sum(i => i.Amount * i.Quantity);

    [NotMapped]
    public long VatAmount => Items.Sum(i => (long)Math.Round(i.Amount * i.VatFactor * i.Quantity));

    [NotMapped]
    public long AfterVatAmount => BeforeVatAmount + VatAmount;
    
    [NotMapped]
    public DateTime? LastUpdatedDateTime => UpdateHistories
        .OrderBy(uh => uh.UpdatedDateTime)
        .Select(uh => uh.UpdatedDateTime)
        .LastOrDefault();

    [NotMapped]
    public User LastUpdatedUser => UpdateHistories
        .OrderBy(uh => uh.UpdatedDateTime)
        .Select(uh => uh.User)
        .LastOrDefault();
}