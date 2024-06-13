namespace NATSInternal.Services.Entities;

public class ExpensePayee
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Navigation properties
    public virtual List<Expense> Expenses { get; set; }
}