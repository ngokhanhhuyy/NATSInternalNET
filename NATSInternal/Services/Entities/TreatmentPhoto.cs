namespace NATSInternal.Services.Entities;

[Table("treatment_photos")]
public class TreatmentPhoto
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("url")]
    [Required]
    [StringLength(255)]
    public string Url { get; set; }

    [Column("treatment_photo_type")]
    [Required]
    public TreatmentPhotoType Type { get; set; }

    // Foreign key
    [Column("treatment_id")]
    [Required]
    public int TreatmentId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Relationships
    public virtual Treatment Treatment { get; set; }
}