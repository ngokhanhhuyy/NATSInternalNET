namespace NATSInternal.Services;

public class DatabaseContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>,
    UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public DbSet<Country> Countries { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Supply> Supplies { get; set; }
    public DbSet<SupplyItem> SupplyItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderPayment> OrderPayments { get; set; }
    public DbSet<Treatment> Treatments { get; set; }
    public DbSet<TreatmentSession> TreatmentSessions { get; set; }
    public DbSet<TreatmentItem> TreatmentItems { get; set; }
    public DbSet<TreatmentPayment> TreatmentPayments { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<ExpensePayee> ExpensePayees { get; set; }
    public DbSet<ExpensePhoto> ExpensePhotos { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    public DbSet<DailyStats> DailyStats { get; set; }
    public DbSet<MonthlyStats> MonthlyStats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(e =>
        {
            e.ToTable("customers");
            e.HasOne(c => c.Introducer)
                .WithMany(i => i.IntroducedCustomers)
                .HasForeignKey(c => c.IntroducerId)
                .HasConstraintName("FK__customers__customers__introducer_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(c => c.CreatedUser)
                .WithMany(u => u.CreatedCustomers)
                .HasForeignKey(c => c.CreatedUserId)
                .HasConstraintName("FK__customers__users__created_user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<ProductCategory>(e =>
        {
            e.ToTable("product_categories");
            e.HasIndex(pc => pc.Name)
                .IsUnique()
                .HasDatabaseName("UX__product_categories__name");
        });
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("products");
            e.HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .HasConstraintName("FK__products__brands__brand_id")
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(p => p.Category)
                .WithMany(pc => pc.Products)
                .HasForeignKey(p => p.CategoryId)
                .HasConstraintName("FK__products__product_categories__category_id")
                .OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(p => p.Name)
                .IsUnique()
                .HasDatabaseName("UX__products__name");
            e.Property(ex => ex.VatFactor)
                .HasPrecision(18, 2);
        });
        modelBuilder.Entity<ProductPhoto>(e =>
        {
            e.ToTable("product_photos");
            e.HasOne(photo => photo.Product)
                .WithMany(product => product.Photos)
                .HasForeignKey(photo => photo.ProductId)
                .HasConstraintName("FK__product_photos__products__product_id")
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Country>(e =>
        {
            e.ToTable("countries");
            e.HasIndex(c => c.Name).IsUnique();
            e.HasIndex(c => c.Code).IsUnique();
        });
        modelBuilder.Entity<Brand>(e =>
        {
            e.ToTable("brands");
            e.HasOne(b => b.Country)
                .WithMany(c => c.Brands)
                .HasForeignKey(b => b.CountryId)
                .HasConstraintName("FK__brands__countries__country_id")
                .OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(b => b.Name)
                .IsUnique()
                .HasDatabaseName("UX__brands__name");
        });
        modelBuilder.Entity<Supply>(e =>
        {
            e.ToTable("supplies");
            e.HasOne(s => s.User)
                .WithMany(u => u.Supplies)
                .HasForeignKey(s => s.UserId)
                .HasConstraintName("FK__supplies__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(s => s.SuppliedDateTime)
                .IsUnique()
                .HasDatabaseName("UX__supply_supplied_datetime");
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<SupplyItem>(e =>
        {
            e.ToTable("supply_items");
            e.HasOne(si => si.Supply)
                .WithMany(s => s.Items)
                .HasForeignKey(si => si.SupplyId)
                .HasConstraintName("FK__supply_items__supplies__supply_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(si => si.Product)
                .WithMany(p => p.SupplyItems)
                .HasForeignKey(si => si.ProductId)
                .HasConstraintName("FK__supply_items__products__product_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(si => si.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<SupplyPhoto>(e =>
        {
            e.ToTable("supply_photos");
            e.HasKey(p => p.Id);
            e.HasOne(p => p.Supply)
                .WithMany(s => s.Photos)
                .HasForeignKey(p => p.SupplyId)
                .HasConstraintName("FK__supply_photos__supplies__supply_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<SupplyUpdateHistories>(e =>
        {
            e.ToTable("supply_update_histories");
            e.HasKey(suh => suh.Id);
            e.HasOne(suh => suh.Supply)
                .WithMany(s => s.UpdateHistories)
                .HasForeignKey(suh => suh.SupplyId)
                .HasConstraintName("FK__supply_update_histories__supplies__supply_id")
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Order>(e =>
        {
            e.ToTable("orders");
            e.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .HasConstraintName("FK__orders__customers__customer_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .HasConstraintName("FK__orders__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(c => c.RowVersion)
                .IsRowVersion();
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<OrderItem>(e =>
        {
            e.ToTable("order_items");
            e.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .HasConstraintName("FK__order_items__orders__order_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .HasConstraintName("FK__order_items__products__product_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(ex => ex.VatFactor)
                .HasPrecision(18, 2);
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<OrderPayment>(e =>
        {
            e.ToTable("order_payments");
            e.HasOne(op => op.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(op => op.OrderId)
                .HasConstraintName("FK__order_payments__orders__order_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(op => op.User)
                .WithMany(u => u.OrderPayments)
                .HasForeignKey(op => op.UserId)
                .HasConstraintName("FK__order_payments__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<OrderPhoto>(e =>
        {
            e.ToTable("order_photos");
            e.HasOne(p => p.Order)
                .WithMany(o => o.Photos)
                .HasForeignKey(p => p.OrderId)
                .HasConstraintName("FK__order_photos__orders__order_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(p => p.Url)
                .IsUnique()
                .HasDatabaseName("UX__order_photos__url");
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<Treatment>(e =>
        {
            e.ToTable("treatments");
            e.HasOne(t => t.User)
                .WithMany(u => u.Treatments)
                .HasForeignKey(t => t.UserId)
                .HasConstraintName("FK__treatments__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.Customer)
                .WithMany(c => c.Treatments)
                .HasForeignKey(t => t.CustomerId)
                .HasConstraintName("FK__treatments__customers__customer_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(ex => ex.VatFactor)
                .HasPrecision(18, 2);
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<TreatmentSession>(e =>
        {
            e.ToTable("treatment_sessions");
            e.HasOne(ts => ts.Treatment)
                .WithMany(t => t.Sessions)
                .HasForeignKey(ts => ts.TreatmentId)
                .HasConstraintName("FK__treatment_sessions__treatments__treatment_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasMany(ts => ts.Therapists)
                .WithMany(u => u.TreatmentSessions)
                .UsingEntity(j => j.ToTable("treatment_session_therapists"));
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<TreatmentItem>(e =>
        {
            e.ToTable("treatment_items");
            e.HasOne(ti => ti.Session)
                .WithMany(ts => ts.Items)
                .HasForeignKey(ti => ti.SessionId)
                .HasConstraintName("FK__treatment_items__treatment_sessions__session_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(ti => ti.Product)
                .WithMany(p => p.TreatmentItems)
                .HasForeignKey(ti => ti.ProductId)
                .HasConstraintName("FK__treatment_items__products__product_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(ex => ex.VatFactor)
                .HasPrecision(18, 2);
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<TreatmentPayment>(e =>
        {
            e.ToTable("treatment_payments");
            e.HasOne(tp => tp.Treatment)
                .WithMany(t => t.Payments)
                .HasForeignKey(tp => tp.TreatmentId)
                .HasConstraintName("FK__treatment_payments__treatments__treatment_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(tp => tp.User)
                .WithMany(u => u.TreatmentPayments)
                .HasForeignKey(tp => tp.UserId)
                .HasConstraintName("FK__treatment_payments__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<TreatmentPhoto>(e =>
        {
            e.ToTable("treatment_photos");
            e.HasOne(p => p.Treatment)
                .WithMany(t => t.Photos)
                .HasForeignKey(p => p.TreatmentId)
                .HasConstraintName("FK__treatment_photos__treatments__treatment_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(p => p.Url)
                .IsUnique()
                .HasDatabaseName("UX__treatment_photos__url");
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<Expense>(e =>
        {
            e.ToTable("expenses");
            e.HasOne(ex => ex.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(ex => ex.UserId)
                .HasConstraintName("FK__expenses__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(ex => ex.Payee)
                .WithMany(exp => exp.Expenses)
                .HasForeignKey(exp => exp.PayeeId)
                .HasConstraintName("FK__expenses__expense_payees__payee_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<ExpensePayee>(e =>
        {
            e.ToTable("expenses_payees");
            e.HasIndex(ep => ep.Name)
                .IsUnique()
                .HasDatabaseName("UX__expense_payees__name");
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<ExpensePhoto>(e =>
        {
            e.ToTable("expense_photos");
            e.HasOne(p => p.Expense)
                .WithMany(ex => ex.Photos)
                .HasForeignKey(ex => ex.ExpenseId)
                .HasConstraintName("FK__expense_photos__expenses__expense_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(p => p.Url)
                .IsUnique()
                .HasDatabaseName("UX__expense_photos__url");
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<Announcement>(e =>
        {
            e.HasOne(a => a.User)
                .WithMany(u => u.Announcements)
                .HasForeignKey(a => a.UserId)
                .HasConstraintName("FK__announcements__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<UserRefreshToken>(e =>
        {
            e.ToTable("user_refresh_tokens");
            e.HasOne(urt => urt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(urt => urt.UserId)
                .HasConstraintName("FK__user_refresh_tokens__users__user_id")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DailyStats>(e =>
        {
            e.ToTable("daily_stats");
            e.HasOne(dfs => dfs.Monthly)
                .WithMany(mfs => mfs.DailyStats)
                .HasForeignKey(dfs => dfs.MonthlyStatsId)
                .HasConstraintName("FK__daily_stats__monthly_stats__monthly_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(dfs => dfs.RecordedDate)
                .IsUnique()
                .HasDatabaseName("UX__daily_stats__recorded_date");
        });

        modelBuilder.Entity<MonthlyStats>(e =>
        {
            e.ToTable("monthly_stats");
            e.HasIndex(dfs => new { dfs.RecordedMonth, dfs.RecordedYear })
                .IsUnique()
                .HasDatabaseName("UX_monthly_stats__recorded_month__recorded_year");
        });

        // Identity entities
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<UserRole>(
                    userRole => userRole
                        .HasOne(ur => ur.Role)
                        .WithMany()
                        .HasForeignKey(ur => ur.RoleId)
                        .HasConstraintName("FK__user_roles__roles__role_id"),
                    userRole => userRole
                        .HasOne(ur => ur.User)
                        .WithMany()
                        .HasForeignKey(ur => ur.UserId)
                        .HasConstraintName("FK__user_roles__users__user_id"),
                    ur => ur.ToTable("user_roles"));
            e.HasIndex(u => u.UserName)
                .IsUnique()
                .HasDatabaseName("UX__users__username");
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasColumnName("id");
            e.Property(u => u.UserName).HasColumnName("username");
            e.Property(u => u.AccessFailedCount).HasColumnName("access_failed_count");
            e.Property(u => u.ConcurrencyStamp).HasColumnName("concurrent_stamp");
            e.Property(u => u.Email).HasColumnName("email");
            e.Property(u => u.EmailConfirmed).HasColumnName("email_confirmed");
            e.Property(u => u.LockoutEnabled).HasColumnName("lockout_enabled");
            e.Property(u => u.LockoutEnd).HasColumnName("lockout_end");
            e.Property(u => u.NormalizedEmail).HasColumnName("normalized_email");
            e.Property(u => u.NormalizedUserName).HasColumnName("normalized_username");
            e.Property(u => u.PasswordHash).HasColumnName("password_hash");
            e.Property(u => u.PhoneNumber).HasColumnName("phone_number");
            e.Property(u => u.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
            e.Property(u => u.SecurityStamp).HasColumnName("security_stamp");
            e.Property(u => u.TwoFactorEnabled).HasColumnName("two_factor_enabled");
            e.Property(u => u.SecurityStamp).HasColumnName("security_stamp");
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<Role>(e =>
        {
            e.ToTable("roles");
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).HasColumnName("id");
            e.Property(r => r.Name).HasColumnName("name");
            e.Property(r => r.NormalizedName).HasColumnName("normalized_name");
            e.Property(r => r.ConcurrencyStamp).HasColumnName("concurrent_stamp");
            e.HasIndex(r => r.Name)
                .IsUnique()
                .HasDatabaseName("UX__roles__name");
            e.HasIndex(r => r.DisplayName)
                .IsUnique()
                .HasDatabaseName("UX__roles__display_name");
        });
        modelBuilder.Entity<UserRole>(e =>
        {
            e.Property(ur => ur.UserId).HasColumnName("user_id");
            e.Property(ur => ur.RoleId).HasColumnName("role_id");
        });
        modelBuilder.Entity<IdentityUserClaim<int>>(e =>
        {
            e.ToTable("user_claims");
            e.Property(uc => uc.Id).HasColumnName("id");
            e.Property(uc => uc.UserId).HasColumnName("user_id");
            e.Property(uc => uc.ClaimType).HasColumnName("claim_type");
            e.Property(uc => uc.ClaimValue).HasColumnName("claim_value");
        });
        modelBuilder.Entity<IdentityUserLogin<int>>(e =>
        {
            e.ToTable("user_logins");
            e.HasKey(ul => ul.UserId);
            e.Property(ul => ul.UserId).HasColumnName("user_id");
            e.Property(ul => ul.LoginProvider).HasColumnName("login_providers");
            e.Property(ul => ul.ProviderDisplayName).HasColumnName("provider_display_name");
            e.Property(ul => ul.ProviderKey).HasColumnName("provider_key");
        });
        modelBuilder.Entity<IdentityUserToken<int>>(e =>
        {
            e.ToTable("user_tokens");
            e.HasKey(ut => ut.UserId);
        });
        modelBuilder.Entity<IdentityRoleClaim<int>>(e =>
        {
            e.ToTable("role_claims");
            e.Property(rc => rc.Id).HasColumnName("id");
            e.Property(rc => rc.ClaimType).HasColumnName("claim_type");
            e.Property(rc => rc.ClaimValue).HasColumnName("claim_value");
            e.Property(rc => rc.RoleId).HasColumnName("role_id");
        });
    }
}