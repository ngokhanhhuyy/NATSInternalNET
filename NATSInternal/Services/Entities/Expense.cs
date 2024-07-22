namespace NATSInternal.Services.Entities;

[Table("expenses")]
public class Expense
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("amount")]
    [Required]
    public long Amount { get; set; }

    [Column("paid_datetime")]
    [Required]
    public DateTime PaidDateTime { get; set; }
    
    [Column("category")]
    [Required]
    public ExpenseCategory Category { get; set; }

    [Column("note")]
    [StringLength(255)]
    public string Note { get; set; }

    [Column("is_closed")]
    [Required]
    public bool IsClosed { get; set; }

    // Foreign keys
    [Column("created_user_id")]
    [Required]
    public int CreatedUserId { get; set; }

    [Column("payee_id")]
    [Required]
    public int PayeeId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Navigation properties
    public virtual User CreatedUser { get; set; }
    public virtual ExpensePayee Payee { get; set; }
    public virtual List<ExpensePhoto> Photos { get; set; }
    public virtual List<ExpenseUpdateHistory> UpdateHistories { get; set; }

    // Properties for convinience.
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