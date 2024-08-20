namespace NATSInternal.Services;

public class DatabaseContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>,
    UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public DbSet<Country> Countries { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductPhoto> ProductPhotos { get; set; } 
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Supply> Supplies { get; set; }
    public DbSet<SupplyUpdateHistory> supplyUpdateHistories { get; set; }
    public DbSet<SupplyItem> SupplyItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderUpdateHistory> OrderUpdateHistories { get; set; }
    public DbSet<Treatment> Treatments { get; set; }
    public DbSet<TreatmentItem> TreatmentItems { get; set; }
    public DbSet<TreatmentPhoto> TreatmentPhotos { get; set; }
    public DbSet<TreatmentUpdateHistory> TreatmentUpdateHistories { get; set; }
    public DbSet<Consultant> Consultants { get; set; }
    public DbSet<ConsultantUpdateHistory> UpdateHistories { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<ExpensePayee> ExpensePayees { get; set; }
    public DbSet<ExpensePhoto> ExpensePhotos { get; set; }
    public DbSet<ExpenseUpdateHistory> ExpenseUpdateHistories { get; set; }
    public DbSet<Debt> Debts { get; set; }
    public DbSet<DebtUpdateHistory> DebtUpdateHistories { get; set; }
    public DbSet<DebtPayment> DebtPayments { get; set; }
    public DbSet<DebtPaymentUpdateHistory> DebtPaymentUpdateHistories { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    public DbSet<DailyStats> DailyStats { get; set; }
    public DbSet<MonthlyStats> MonthlyStats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(e =>
        {
            e.ToTable("customers");
            e.HasKey(c => c.Id);
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
            e.HasKey(pc => pc.Id);
            e.HasIndex(pc => pc.Name)
                .IsUnique()
                .HasDatabaseName("UX__product_categories__name");
        });
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("products");
            e.HasKey(p => p.Id);
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
            e.HasKey(pp => pp.Id);
            e.HasOne(photo => photo.Product)
                .WithMany(product => product.Photos)
                .HasForeignKey(photo => photo.ProductId)
                .HasConstraintName("FK__product_photos__products__product_id")
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Country>(e =>
        {
            e.ToTable("countries");
            e.HasKey(c => c.Id);
            e.HasIndex(c => c.Name)
                .IsUnique()
                .HasDatabaseName("UX__countries__name");
            e.HasIndex(c => c.Code)
                .IsUnique()
                .HasDatabaseName("UX__countries__code");
        });
        modelBuilder.Entity<Brand>(e =>
        {
            e.ToTable("brands");
            e.HasKey(b => b.Id);
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
            e.HasKey(s => s.Id);
            e.HasOne(s => s.CreatedUser)
                .WithMany(u => u.Supplies)
                .HasForeignKey(s => s.CreatedUserId)
                .HasConstraintName("FK__supplies__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(s => s.PaidDateTime)
                .IsUnique()
                .HasDatabaseName("UX__supply_paid_datetime");
            e.HasIndex(s => s.IsDeleted)
                .HasDatabaseName("IX__supplies__is_deleted");
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<SupplyItem>(e =>
        {
            e.ToTable("supply_items");
            e.HasKey(si => si.Id);
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
        modelBuilder.Entity<SupplyUpdateHistory>(e =>
        {
            e.ToTable("supply_update_histories");
            e.HasKey(suh => suh.Id);
            e.HasOne(suh => suh.Supply)
                .WithMany(s => s.UpdateHistories)
                .HasForeignKey(suh => suh.SupplyId)
                .HasConstraintName("FK__supply_update_histories__supplies__supply_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(suh => suh.User)
                .WithMany(u => u.SupplyUpdateHistories)
                .HasForeignKey(suh => suh.UserId)
                .HasConstraintName("FK__supply_update_histories__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(suh => suh.UpdatedDateTime)
                .HasDatabaseName("IX__supply_update_histories__updated_datetime");
        });
        modelBuilder.Entity<Order>(e =>
        {
            e.ToTable("orders");
            e.HasKey(o => o.Id);
            e.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .HasConstraintName("FK__orders__customers__customer_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(o => o.CreatedUser)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.CreatedUserId)
                .HasConstraintName("FK__orders__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(o => o.PaidDateTime)
                .HasDatabaseName("IX__orders__paid_datetime");
            e.HasIndex(o => o.IsDeleted)
                .HasDatabaseName("IX__orders__is_deleted");
            e.Property(c => c.RowVersion)
                .IsRowVersion();
            e.Property(c => c.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<OrderItem>(e =>
        {
            e.ToTable("order_items");
            e.HasKey(oi => oi.Id);
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
        modelBuilder.Entity<OrderPhoto>(e =>
        {
            e.ToTable("order_photos");
            e.HasKey(op => op.Id);
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
        modelBuilder.Entity<OrderUpdateHistory>(e =>
        {
            e.ToTable("order_update_histories");
            e.HasKey(ouh => ouh.Id);
            e.HasOne(ouh => ouh.Order)
                .WithMany(o => o.UpdateHistories)
                .HasForeignKey(ouh => ouh.OrderId)
                .HasConstraintName("FK__order_update_histories__orders__order_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(ouh => ouh.User)
                .WithMany(u => u.OrderUpdateHistories)
                .HasForeignKey(ouh => ouh.UserId)
                .HasConstraintName("FK__order_update_histories__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(ouh => ouh.UpdatedDateTime)
                .HasDatabaseName("IX__order_update_histories__updated_datetime");
        });
        modelBuilder.Entity<Treatment>(e =>
        {
            e.ToTable("treatments");
            e.HasKey(t => t.Id);
            e.HasOne(t => t.CreatedUser)
                .WithMany(u => u.CreatedTreatments)
                .HasForeignKey(t => t.CreatedUserId)
                .HasConstraintName("FK__treatments__users__created_user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.Therapist)
                .WithMany(u => u.TreatmentsInCharge)
                .HasForeignKey(t => t.TherapistId)
                .HasConstraintName("FK__treatments__users__therapist_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.Customer)
                .WithMany(c => c.Treatments)
                .HasForeignKey(t => t.CustomerId)
                .HasConstraintName("FK__treatments__customers__customer_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(t => t.PaidDateTime)
                .HasDatabaseName("IX__treatments__ordered_datetime");
            e.HasIndex(t => t.IsDeleted)
                .HasDatabaseName("IX__treatments__is_deleted");
            e.Property(t => t.ServiceVatFactor)
                .HasPrecision(18, 2);
            e.Property(t => t.RowVersion)
                .IsRowVersion();
        });
        modelBuilder.Entity<TreatmentItem>(e =>
        {
            e.ToTable("treatment_items");
            e.HasKey(ti => ti.Id);
            e.HasOne(ti => ti.Treatment)
                .WithMany(t => t.Items)
                .HasForeignKey(ti => ti.TreatmentId)
                .HasConstraintName("FK__treatment_items__treatments__treatment_id")
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
        modelBuilder.Entity<TreatmentPhoto>(e =>
        {
            e.ToTable("treatment_photos");
            e.HasKey(tp => tp.Id);
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
        modelBuilder.Entity<TreatmentUpdateHistory>(e =>
        {
            e.ToTable("treatment_update_histories");
            e.HasKey(tuh => tuh.Id);
            e.HasOne(tuh => tuh.Treatment)
                .WithMany(t => t.UpdateHistories)
                .HasForeignKey(tuh => tuh.TreatmentId)
                .HasConstraintName("FK__treatment_update_histories__treatment__treatment_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(tuh => tuh.User)
                .WithMany(u => u.TreatmentUpdateHistories)
                .HasForeignKey(tuh => tuh.UserId)
                .HasConstraintName("FK__treatment_update_histories__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(tuh => tuh.UpdatedDateTime)
                .HasDatabaseName("IX__treatment_update_histories__updated_datetime");
        });
        modelBuilder.Entity<Consultant>(e =>
        {
            e.ToTable("consultants");
            e.HasKey(c => c.Id);
            e.HasOne(cst => cst.Customer)
                .WithMany(ctm => ctm.Consultants)
                .HasForeignKey(cst => cst.CustomerId)
                .HasConstraintName("FK__consultants__customers__customer_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(cst => cst.CreatedUser)
                .WithMany(u => u.Consultants)
                .HasForeignKey(cst => cst.CreatedUserId)
                .HasConstraintName("FK__consultants__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(cst => cst.IsDeleted)
                .HasDatabaseName("IX__consultants__is_deleted");
        });
        modelBuilder.Entity<ConsultantUpdateHistory>(e =>
        {
            e.ToTable("consultant_update_histories");
            e.HasKey(cuh => cuh.Id);
            e.HasOne(cuh => cuh.Consultant)
                .WithMany(c => c.UpdateHistories)
                .HasForeignKey(cuh => cuh.ConsultantId)
                .HasConstraintName("FK__consultant_update_histories__consultants__consultant_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(cuh => cuh.User)
                .WithMany(u => u.ConsultantUpdateHistories)
                .HasForeignKey(cuh => cuh.UserId)
                .HasConstraintName("FK__consultant_update_histories__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(cuh => cuh.UpdatedDateTime)
                .HasDatabaseName("IX__consultant_update_histories__updated_datetime");
        });
        modelBuilder.Entity<Expense>(e =>
        {
            e.ToTable("expenses");
            e.HasKey(ex => ex.Id);
            e.HasOne(ex => ex.CreatedUser)
                .WithMany(u => u.Expenses)
                .HasForeignKey(ex => ex.CreatedUserId)
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
            e.HasKey(ep => ep.Id);
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
        modelBuilder.Entity<ExpenseUpdateHistory>(e =>
        {
            e.ToTable("expense_update_histories");
            e.HasKey(euh => euh.Id);
            e.HasOne(euh => euh.Expense)
                .WithMany(ex => ex.UpdateHistories)
                .HasForeignKey(euh => euh.ExpenseId)
                .HasConstraintName("FK__expense_update_histories__expenses__expense_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(euh => euh.User)
                .WithMany(u => u.ExpenseUpdateHistories)
                .HasForeignKey(euh => euh.UserId)
                .HasConstraintName("FK__expense_update_histories__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(euh => euh.UpdatedDateTime)
                .HasDatabaseName("IX__expense_update_histories__updated_datetime");
        });
        modelBuilder.Entity<Debt>(e =>
        {
            e.ToTable("debts");
            e.HasKey(d => d.Id);
            e.HasOne(d => d.Customer)
                .WithMany(c => c.Debts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__debts__customers__customer_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.CreatedUser)
                .WithMany(u => u.Debts)
                .HasForeignKey(d => d.CreatedUserId)
                .HasConstraintName("FK__debts__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(d => d.CreatedDateTime)
                .HasDatabaseName("IX__debts__incurred_datetime");
            e.HasIndex(d => d.IsDeleted)
                .HasDatabaseName("IX__debts__is_deleted");
        });
        modelBuilder.Entity<DebtUpdateHistory>(e =>
        {
            e.ToTable("debt_update_histories");
            e.HasKey(duh => duh.Id);
            e.HasOne(duh => duh.Debt)
                .WithMany(d => d.UpdateHistories)
                .HasForeignKey(duh => duh.DebtId)
                .HasConstraintName("FK__debt_update_histories__debts__debt_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(duh => duh.User)
                .WithMany(u => u.DebtUpdateHistories)
                .HasForeignKey(duh => duh.UserId)
                .HasConstraintName("FK__debt_update_histories__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(duh => duh.UpdatedDateTime)
                .HasDatabaseName("IX__debt_update_histories__updated_datetime");
        });
        modelBuilder.Entity<DebtPayment>(e =>
        {
            e.ToTable("debt_payments");
            e.HasKey(dp => dp.Id);
            e.HasOne(dp => dp.Customer)
                .WithMany(c => c.DebtPayments)
                .HasForeignKey(dp => dp.CustomerId)
                .HasConstraintName("FK__debt_payments__customers__customer_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(dp => dp.CreatedUser)
                .WithMany(u => u.DebtPayments)
                .HasForeignKey(dp => dp.CreatedUserId)
                .HasConstraintName("FK__debt_payments__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(dp => dp.PaidDateTime)
                .HasDatabaseName("IX__debt_payments__paid_datetime");
            e.HasIndex(d => d.IsDeleted)
                .HasDatabaseName("IX__debt_payments__is_deleted");
        });
        modelBuilder.Entity<DebtPaymentUpdateHistory>(e =>
        {
            e.ToTable("debt_payment_update_history");
            e.HasKey(dpuh => dpuh.Id);
            e.HasOne(dpuh => dpuh.DebtPayment)
                .WithMany(dp => dp.UpdateHistories)
                .HasForeignKey(dp => dp.DebtPaymentId)
                .HasConstraintName("FK__debt_payment_update_histories__debt_payments__debt_payment_id")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(dpuh => dpuh.User)
                .WithMany(u => u.DebtPaymentUpdateHistories)
                .HasForeignKey(dpuh => dpuh.UserId)
                .HasConstraintName("FK__debt_paymetn_update_histories__users__user_id")
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(dpuh => dpuh.UpdatedDateTime)
                .HasDatabaseName("IX__debt_payment_update_histories__updated_datetime");
        });
        modelBuilder.Entity<Announcement>(e =>
        {
            e.ToTable("announcements");
            e.HasKey(a => a.Id);
            e.HasOne(a => a.CreatedUser)
                .WithMany(u => u.Announcements)
                .HasForeignKey(a => a.CreatedUserId)
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
            e.HasKey(ms => ms.Id);
            e.HasIndex(dfs => new { dfs.RecordedMonth, dfs.RecordedYear })
                .IsUnique()
                .HasDatabaseName("UX_monthly_stats__recorded_month__recorded_year");
        });
        // Identity entities
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(u => u.Id);
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
            e.HasKey(ur => new { ur.UserId, ur.RoleId });
            e.Property(ur => ur.UserId).HasColumnName("user_id");
            e.Property(ur => ur.RoleId).HasColumnName("role_id");
        });
        modelBuilder.Entity<IdentityUserClaim<int>>(e =>
        {
            e.ToTable("user_claims");
            e.HasKey(uc => uc.Id);
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
            e.HasKey(rc => rc.Id);
            e.Property(rc => rc.Id).HasColumnName("id");
            e.Property(rc => rc.ClaimType).HasColumnName("claim_type");
            e.Property(rc => rc.ClaimValue).HasColumnName("claim_value");
            e.Property(rc => rc.RoleId).HasColumnName("role_id");
        });
    }
}