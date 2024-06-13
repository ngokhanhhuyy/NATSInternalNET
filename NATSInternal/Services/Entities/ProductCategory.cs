namespace NATSInternal.Services.Entities;

[Table("product_category")]
public class ProductCategory
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("name")]
    [Required]
    [StringLength(30)]
    public string Name { get; set; }

    [Column("created_datetime")]
    [Required]
    public DateTime CreatedDateTime { get; set; } = DateTime.Now;

    // Relationships
    public virtual List<Product> Products { get; set; }
}