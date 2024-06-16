namespace NATSInternal.Services.Entities;

[Table("treatment_payments")]
public class TreatmentPayment
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("paid_amount")]
    [Required]
    public long PaidAmount { get; set; }

    [Column("paid_datetime")]
    [Required]
    public DateTime PaidDateTime { get; set; }

    [Column("note")]
    [StringLength(255)]
    public string Note { get; set; }

    [Column("is_closed")]
    [Required]
    public bool IsClosed { get; set; } = false;

    // Foreign keys
    [Column("treatment_id")]
    [Required]
    public int TreatmentId { get; set; }

    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Relationships
    public virtual Treatment Treatment { get; set; }
    public virtual User User { get; set; }
}