namespace NATSInternal.Services.Entities;

[Table("supply_update_histories")]
public class SupplyUpdateHistory
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("updated_datetime")]
    [Required]
    public DateTime UpdatedDateTime { get; set; } = DateTime.UtcNow.ToApplicationTime();

    [Column("reason")]
    [StringLength(255)]
    public string Reason { get; set; }

    [Column("old_data", TypeName = "JSON")]
    [StringLength(1000)]
    public string OldData { get; set; }

    [Column("new_data", TypeName = "JSON")]
    [Required]
    [StringLength(1000)]
    public string NewData { get; set; }

    // Foreign keys
    [Column("supply_id")]
    [Required]
    public int SupplyId { get; set; }

    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    // Navigation properties
    public virtual Supply Supply { get; set; }
    public virtual User User { get; set; }
}
