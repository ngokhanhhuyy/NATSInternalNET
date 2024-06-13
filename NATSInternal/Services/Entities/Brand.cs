namespace NATSInternal.Services.Entities;

public class Brand
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("name")]
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Column("website")]
    [StringLength(255)]
    public string Website { get; set; }

    [Column("social_media_url")]
    [StringLength(1000)]
    public string SocialMediaUrl { get; set; }

    [Column("phone_number")]
    [StringLength(15)]
    public string PhoneNumber { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; }

    [Column("address")]
    [StringLength(255)]
    public string Address { get; set; }

    [Column("thumbnail_url")]
    [StringLength(255)]
    public string ThumbnailUrl { get; set; }

    // Foreign keys
    [Column("country_id")]
    public int? CountryId { get; set; }

    // Relationships
    public virtual List<Product> Products { get; set; }
    public virtual Country Country { get; set; }
}