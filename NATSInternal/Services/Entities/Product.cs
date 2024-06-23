namespace NATSInternal.Services.Entities;

[Table("products")]
public class Product
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("name")]
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Column("description")]
    [StringLength(1000)]
    public string Description { get; set; }

    [Column("unit")]
    [Required]
    [StringLength(12)]
    public string Unit { get; set; }

    [Column("price")]
    [Required]
    public long Price { get; set; }

    [Column("var_factor")]
    [Required]
    public decimal VatFactor { get; set; } = 0.1M;

    [Column("is_for_retail")]
    [Required]
    public bool IsForRetail { get; set; } = true;

    [Column("is_discontinued")]
    [Required]
    public bool IsDiscontinued { get; set; } = false;

    [Column("created_datetime")]
    [Required]
    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow.ToApplicationTime();
    
    [Column("updated_datetime")]
    public DateTime? UpdatedDateTime { get; set; } = DateTime.UtcNow.ToApplicationTime();
    
    [Column("thumbnail_url")]
    [StringLength(255)]
    public string ThumbnailUrl { get; set; }

    [Column("stocking_quantity")]
    [Required]
    public int StockingQuantity { get; set; }

    [Column("is_deleted")]
    [Required]
    public bool IsDeleted = false;

    // Foreign keys
    [Column("brand_id")]
    public int? BrandId { get; set; }
    
    [Column("category_id")]
    public int? CategoryId { get; set; }

    // Relationships
    public virtual Brand Brand { get; set; }
    public virtual ProductCategory Category { get; set; }
    public virtual List<SupplyItem> SupplyItems { get; set; }
    public virtual List<OrderItem> OrderItems { get; set; }
    public virtual List<TreatmentItem> TreatmentItems { get; set; }
    public virtual List<ProductPhoto> Photos { get; set; }
}