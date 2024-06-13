namespace NATSInternal.Services.Entities;

[Table("roles")]
public class Role : IdentityRole<int>
{
    [Column("display_name")]
    [Required]
    [StringLength(50)]
    public string DisplayName { get; set; }

    [Column("power_level")]
    [Required]
    public int PowerLevel { get; set; }

    public virtual List<User> Users { get; set; }
    public virtual List<IdentityRoleClaim<int>> Claims { get; set; }
}