namespace NATSInternal.Services.Entities;

[Table("expense_categories")]
public class ExpenseCategory
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("name")]
    [Required]
    [StringLength(30)]
    public string Name { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Navigation properties
    public virtual List<Expense> Expenses { get; set; }
}