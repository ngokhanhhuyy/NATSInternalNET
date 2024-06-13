namespace NATSInternal.Services.Entities;

[Table("expense_photo")]
public class ExpensePhoto
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("url")]
    [Required]
    [StringLength(255)]
    public string Url { get; set; }

    // Foreign key
    [Column("expense_id")]
    [Required]
    public int ExpenseId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Navigation properties
    public virtual Expense Expense { get; set; }
}