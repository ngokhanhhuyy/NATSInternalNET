namespace NATSInternal.Services.Entities;

[Table("treatment_sessions")]
public class TreatmentSession
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("starting_datetime")]
    [Required]
    public DateTime StartingDateTime { get; set; }
    
    [Column("ending_datetime")]
    public DateTime? EndingDateTime { get; set; }

    [Column("is_closed")]
    [Required]
    public bool IsClosed { get; set; } = false;

    // Foreign key
    [Column("treatment_id")]
    [Required]
    public int TreatmentId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Relationship
    public virtual Treatment Treatment { get; set; }
    public virtual List<TreatmentItem> Items { get; set; }
    public virtual List<User> Therapists { get; set; }
}