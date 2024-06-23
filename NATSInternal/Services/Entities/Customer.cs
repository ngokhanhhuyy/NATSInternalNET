namespace NATSInternal.Services.Entities;

[Table("customers")]
public class Customer
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("first_name")]
    [Required]
    [StringLength(10)]
    public string FirstName { get; set; }

    [Column("normalized_first_name")]
    [Required]
    [StringLength(10)]
    public required string NormalizedFirstName { get; set; }

    [Column("middle_name")]
    [StringLength(20)]
    public string MiddleName { get; set; }

    [Column("normalized_middle_name")]
    [StringLength(20)]
    public string NormalizedMiddleName { get; set; }

    [Column("last_name")]
    [Required]
    [StringLength(10)]
    public string LastName { get; set; }

    [Column("normalized_last_name")]
    [Required]
    [StringLength(10)]
    public required string NormalizedLastName { get; set; }

    [Column("fullname")]
    [Required]
    [StringLength(45)]
    public string FullName { get; set; }

    [Column("normalized_full_name")]
    [Required]
    [StringLength(45)]
    public required string NormalizedFullName { get; set; }

    [Column("nickname")]
    [StringLength(35)]
    public string NickName { get; set; }

    [Column("gender")]
    [Required]
    public Gender Gender { get; set; }

    [Column("birthday")]
    public DateOnly? Birthday { get; set; }

    [Column("phone_number")]
    [StringLength(15)]
    public string PhoneNumber { get; set; }

    [Column("zalo_number")]
    [StringLength(15)]
    public string ZaloNumber { get; set; }

    [Column("facebook_url")]
    [StringLength(1000)]
    public string FacebookUrl { get; set; }

    [Column("email")]
    [StringLength(320)]
    public string Email { get; set; }

    [Column("address")]
    [StringLength(255)]
    public string Address { get; set; }

    [Column("created_datetime")]
    [Required]
    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow.ToApplicationTime();

    [Column("updated_datetime")]
    public DateTime? UpdatedDateTime { get; set; }

    [Column("note")]
    [StringLength(255)]
    public string Note { get; set; }

    [Column("is_deleted")]
    [Required]
    public bool IsDeleted { get; set; } = false;

    // Foreign keys
    [Column("introducer_id")]
    public int? IntroducerId { get; set; }

    [Required]
    [Column("created_user_id")]
    public int CreatedUserId { get; set; }

    // Concurrency operation tracking field
    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Relationships
    public virtual User CreatedUser { get; set; }
    public virtual List<Customer> IntroducedCustomers { get; set; }
    public virtual Customer Introducer { get; set; }
    public virtual List<Order> Orders { get; set; }
    public virtual List<Treatment> Treatments { get; set; }
}