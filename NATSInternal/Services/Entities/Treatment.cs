namespace NATSInternal.Services.Entities;

[Table("treatments")]
public class Treatment
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("ordered_datetime")]
    [Required]
    public DateTime OrderedDateTime { get; set; }

    [Column("created_datetime")]
    [Required]
    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow.ToApplicationTime();

    [Column("updated_datetime")]
    public DateTime? UpdatedDateTime { get; set; }

    [Column("service_amount")]
    [Required]
    public long ServiceAmount { get; set; } = 0;

    [Column("vat_factor")]
    [Required]
    public decimal VatFactor { get; set; } = 0.1M;

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
    [Column("user_id")]
    public int UserId { get; set; }
    
    [Column("customer_id")]
    public int CustomerId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
    public virtual Customer Customer { get; set; }
    public virtual List<TreatmentSession> Sessions { get; set; }
    public virtual List<TreatmentPayment> Payments { get; set; }
    public virtual List<TreatmentPhoto> Photos { get; set; }

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
    public long ItemAmount => Sessions.Sum(ts => ts.ItemAmount);

    [NotMapped]
    public long TotalAmount => (long)Math.Round(ItemAmount + (Decimal)ServiceAmount * VatFactor);

    [NotMapped]
    public long Dept => TotalAmount - Payments.Sum(tp => tp.PaidAmount);
}