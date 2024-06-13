namespace NATSInternal.Services.Entities;

[Table("product_photos")]
public class ProductPhoto
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("url")]
    [Required]
    [StringLength(255)]
    public string Url { get; set; }

    // Foreign keys
    [Column("product_id")]
    [Required]
    public int ProductId { get; set; }

    // Relationship
    public virtual Product Product { get; set; }
}