namespace NATSInternal.Services.Entities;

[Table("treatments")]
public class Treatment : LockableEntity
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("paid_datetime")]
    [Required]
    public DateTime PaidDateTime { get; set; }

    [Column("service_amount")]
    [Required]
    public long ServiceAmount { get; set; } = 0;

    [Column("service_vat_factor")]
    [Required]
    public decimal ServiceVatFactor { get; set; } = 0.1M;

    [Column("note")]
    [StringLength(255)]
    public string Note { get; set; }

    [Column("is_deleted")]
    [Required]
    public bool IsDeleted { get; set; } = false;

    // Foreign keys
    [Column("created_user_id")]
    [Required]
    public int CreatedUserId { get; set; }

    [Column("therapist_id")]
    [Required]
    public int TherapistId { get; set; }
    
    [Column("customer_id")]
    public int CustomerId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Navigation properties
    public virtual User CreatedUser { get; set; }
    public virtual User Therapist { get; set; }
    public virtual Customer Customer { get; set; }
    public virtual List<TreatmentItem> Items { get; set; }
    public virtual List<TreatmentPhoto> Photos { get; set; }
    public virtual List<TreatmentUpdateHistory> UpdateHistories { get; set; }

    // Properties for convinience
    [NotMapped]
    public List<TreatmentPhoto> PreTreatmentPhotos => Photos?
        .OrderBy(p => p.Id)
        .Where(p => p.Type == TreatmentPhotoType.Before)
        .ToList();

    [NotMapped]
    public List<TreatmentPhoto> PostTreatmentPhotos => Photos?
        .OrderBy(p => p.Id)
        .Where(p => p.Type == TreatmentPhotoType.After)
        .ToList();

    [NotMapped]
    public long ProductAmount => Items.Sum(ts => ts.Amount + ts.Amount * ts.Quantity);

    [NotMapped]
    public long ProductVatAmount => Items.Sum(ts => (long)Math.Round(ts.Amount * ts.VatFactor * ts.Quantity));

    [NotMapped]
    public long ServiceVatAmount => (long)Math.Round(ServiceAmount * ServiceVatFactor);

    [NotMapped]
    public long Amount => ProductAmount + ServiceAmount;

    [NotMapped]
    public long VatAmount => (long)Math.Round(ProductVatAmount + (ServiceAmount * ServiceVatFactor));

    [NotMapped]
    public long TotalAmountAfterVAT => ProductAmount + ProductVatAmount + ServiceAmount + ServiceVatAmount;

    [NotMapped]
    public DateTime? LastUpdatedDateTime => UpdateHistories
        .OrderBy(uh => uh.UpdatedDateTime)
        .Select(uh => (DateTime?)uh.UpdatedDateTime)
        .LastOrDefault();

    [NotMapped]
    public User LastUpdatedUser => UpdateHistories
        .OrderBy(uh => uh.UpdatedDateTime)
        .Select(uh => uh.User)
        .FirstOrDefault();
}