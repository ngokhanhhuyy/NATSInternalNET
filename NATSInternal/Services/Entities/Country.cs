namespace NATSInternal.Services.Entities;

[Table("countries")]
[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Code), IsUnique = true)]
public class Country
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("name")]
    [Required]
    [StringLength(40)]
    public string Name { get; set; }

    [Column("code")]
    [Required]
    [StringLength(3)]
    public string Code { get; set; }

    // Relationships
    public virtual List<Brand> Brands { get; set; }
}