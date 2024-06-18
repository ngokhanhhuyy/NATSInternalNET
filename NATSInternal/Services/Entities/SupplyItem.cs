namespace NATSInternal.Services.Entities;

[Table("supply_items")]
public class SupplyItem
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("amount")]
    [Required]
    public long Amount { get; set; }

    [Column("supplied_quantities")]
    [Required]
    public int SuppliedQuantity { get; set; } = 1;

    // Foreign keys
    [Column("supply_id")]
    [Required]
    public int SupplyId { get; set; }

    [Column("product_id")]
    [Required]
    public int ProductId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    [JsonIgnore]
    public byte[] RowVersion { get; set; }

    // Relationships
    [JsonIgnore]
    public virtual Supply Supply { get; set; }

    [JsonIgnore]
    public virtual Product Product { get; set; }

    [JsonIgnore]
    public virtual List<OrderItem> OrderItems { get; set; }
}