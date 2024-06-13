using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NATSInternal.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "expense_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_categories", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "expenses_payees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expenses_payees", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "product_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_categories", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    display_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    power_level = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    concurrent_stamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    claim_type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    claim_value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_claims", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_logins",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    login_providers = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    provider_key = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    provider_display_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_logins", x => x.user_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_tokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LoginProvider = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tokens", x => x.UserId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    first_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_first_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    middle_name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_middle_name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_last_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    full_name = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_full_name = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    gender = table.Column<int>(type: "int", nullable: false),
                    birthday = table.Column<DateOnly>(type: "date", nullable: true),
                    joining_date = table.Column<DateOnly>(type: "date", nullable: true),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    avatar_url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true),
                    username = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_username = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email_confirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    password_hash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    security_stamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    concurrent_stamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone_number = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone_number_confirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    access_failed_count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    website = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    social_media_url = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone_number = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    address = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    thumbnail_url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    country_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brands", x => x.id);
                    table.ForeignKey(
                        name: "FK__brands__countries__country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "role_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    claim_type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    claim_value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "FK_role_claims_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "announcements",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    category = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    starting_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ending_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_announcements", x => x.id);
                    table.ForeignKey(
                        name: "FK__announcements__users__user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    first_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_first_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    middle_name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_middle_name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_last_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fullname = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_full_name = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nickname = table.Column<string>(type: "varchar(35)", maxLength: 35, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    gender = table.Column<int>(type: "int", nullable: false),
                    birthday = table.Column<DateOnly>(type: "date", nullable: true),
                    phone_number = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    zalo_number = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    facebook_url = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(320)", maxLength: 320, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    address = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    introducer_id = table.Column<int>(type: "int", nullable: true),
                    created_user_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.id);
                    table.ForeignKey(
                        name: "FK__customers__customers__introducer_id",
                        column: x => x.introducer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__customers__users__created_user_id",
                        column: x => x.created_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "expenses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    vat_factor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    paid_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    payee_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expenses", x => x.id);
                    table.ForeignKey(
                        name: "FK__expenses__expense_categories__category_id",
                        column: x => x.category_id,
                        principalTable: "expense_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__expenses__expense_payees__payee_id",
                        column: x => x.payee_id,
                        principalTable: "expenses_payees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__expenses__users__user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "supplies",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    supplied_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    shipment_fee = table.Column<long>(type: "bigint", nullable: false),
                    paid_amount = table.Column<long>(type: "bigint", nullable: false),
                    note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supplies", x => x.id);
                    table.ForeignKey(
                        name: "FK__supplies__users__user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_refresh_tokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    token = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    issued_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    expiring_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK__user_refresh_tokens__users__user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.role_id, x.user_id });
                    table.ForeignKey(
                        name: "FK__user_roles__roles__role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__user_roles__users__user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    unit = table.Column<string>(type: "varchar(12)", maxLength: 12, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<long>(type: "bigint", nullable: false),
                    var_factor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    is_for_retail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_discontinued = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    thumbnail_url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    stocking_quantity = table.Column<int>(type: "int", nullable: false),
                    brand_id = table.Column<int>(type: "int", nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                    table.ForeignKey(
                        name: "FK__products__brands__brand_id",
                        column: x => x.brand_id,
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__products__product_categories__category_id",
                        column: x => x.category_id,
                        principalTable: "product_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ordered_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    shipment_fee = table.Column<long>(type: "bigint", nullable: false),
                    shipment_fee_included = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK__orders__customers__customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__orders__users__user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "treatments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ordered_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    vat_factor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_treatments", x => x.id);
                    table.ForeignKey(
                        name: "FK__treatments__customers__customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__treatments__users__user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "expense_photos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expense_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_photos", x => x.id);
                    table.ForeignKey(
                        name: "FK__expense_photos__expenses__expense_id",
                        column: x => x.expense_id,
                        principalTable: "expenses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "supply_photos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    supply_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supply_photos", x => x.id);
                    table.ForeignKey(
                        name: "FK__supply_photos__supplies__supply_id",
                        column: x => x.supply_id,
                        principalTable: "supplies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "product_photos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    product_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_photos", x => x.id);
                    table.ForeignKey(
                        name: "FK__product_photos__products__product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "supply_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    vat_factor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    supplied_quantities = table.Column<int>(type: "int", nullable: false),
                    supply_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supply_items", x => x.id);
                    table.ForeignKey(
                        name: "FK__supply_items__products__product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__supply_items__supplies__supply_id",
                        column: x => x.supply_id,
                        principalTable: "supplies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "order_payments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    paid_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_payments", x => x.id);
                    table.ForeignKey(
                        name: "FK__order_payments__orders__order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__order_payments__users__user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "order_photos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_photos", x => x.id);
                    table.ForeignKey(
                        name: "FK__order_photos__orders__order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "treatment_payments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    paid_amount = table.Column<long>(type: "bigint", nullable: false),
                    paid_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    note = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    treatment_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_treatment_payments", x => x.id);
                    table.ForeignKey(
                        name: "FK__treatment_payments__treatments__treatment_id",
                        column: x => x.treatment_id,
                        principalTable: "treatments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__treatment_payments__users__user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "treatment_photos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    treatment_photo_type = table.Column<int>(type: "int", nullable: false),
                    treatment_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_treatment_photos", x => x.id);
                    table.ForeignKey(
                        name: "FK__treatment_photos__treatments__treatment_id",
                        column: x => x.treatment_id,
                        principalTable: "treatments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "treatment_sessions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    starting_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ending_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    treatment_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_treatment_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK__treatment_sessions__treatments__treatment_id",
                        column: x => x.treatment_id,
                        principalTable: "treatments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "order_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    vat_factor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true),
                    SupplyItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.id);
                    table.ForeignKey(
                        name: "FK__order_items__orders__order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__order_items__products__product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_items_supply_items_SupplyItemId",
                        column: x => x.SupplyItemId,
                        principalTable: "supply_items",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "treatment_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    vat_factor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    session_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<DateTime>(type: "timestamp(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_treatment_items", x => x.id);
                    table.ForeignKey(
                        name: "FK__treatment_items__products__product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__treatment_items__treatment_sessions__session_id",
                        column: x => x.session_id,
                        principalTable: "treatment_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "treatment_session_therapists",
                columns: table => new
                {
                    TherapistsId = table.Column<int>(type: "int", nullable: false),
                    TreatmentSessionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_treatment_session_therapists", x => new { x.TherapistsId, x.TreatmentSessionsId });
                    table.ForeignKey(
                        name: "FK_treatment_session_therapists_treatment_sessions_TreatmentSes~",
                        column: x => x.TreatmentSessionsId,
                        principalTable: "treatment_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_treatment_session_therapists_users_TherapistsId",
                        column: x => x.TherapistsId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_announcements_user_id",
                table: "announcements",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_brands_country_id",
                table: "brands",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "UX__brands__name",
                table: "brands",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_countries_code",
                table: "countries",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_countries_name",
                table: "countries",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customers_created_user_id",
                table: "customers",
                column: "created_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_customers_introducer_id",
                table: "customers",
                column: "introducer_id");

            migrationBuilder.CreateIndex(
                name: "IX_expense_photos_expense_id",
                table: "expense_photos",
                column: "expense_id");

            migrationBuilder.CreateIndex(
                name: "UX__expense_photos__url",
                table: "expense_photos",
                column: "url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_expenses_category_id",
                table: "expenses",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_payee_id",
                table: "expenses",
                column: "payee_id");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_user_id",
                table: "expenses",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UX__expense_payees__name",
                table: "expenses_payees",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_items_order_id",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_product_id",
                table: "order_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_SupplyItemId",
                table: "order_items",
                column: "SupplyItemId");

            migrationBuilder.CreateIndex(
                name: "IX_order_payments_order_id",
                table: "order_payments",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_payments_user_id",
                table: "order_payments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_photos_order_id",
                table: "order_photos",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "UX__order_photos__url",
                table: "order_photos",
                column: "url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_customer_id",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_user_id",
                table: "orders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UX__product_categories__name",
                table: "product_categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_photos_product_id",
                table: "product_photos",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_brand_id",
                table: "products",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_category_id",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "UX__products__name",
                table: "products",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_claims_role_id",
                table: "role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "UX__roles__display_name",
                table: "roles",
                column: "display_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX__roles__name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_supplies_user_id",
                table: "supplies",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_supply_items_product_id",
                table: "supply_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_supply_items_supply_id",
                table: "supply_items",
                column: "supply_id");

            migrationBuilder.CreateIndex(
                name: "IX_supply_photos_supply_id",
                table: "supply_photos",
                column: "supply_id");

            migrationBuilder.CreateIndex(
                name: "IX_treatment_items_product_id",
                table: "treatment_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_treatment_items_session_id",
                table: "treatment_items",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_treatment_payments_treatment_id",
                table: "treatment_payments",
                column: "treatment_id");

            migrationBuilder.CreateIndex(
                name: "IX_treatment_payments_user_id",
                table: "treatment_payments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_treatment_photos_treatment_id",
                table: "treatment_photos",
                column: "treatment_id");

            migrationBuilder.CreateIndex(
                name: "UX__treatment_photos__url",
                table: "treatment_photos",
                column: "url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_treatment_session_therapists_TreatmentSessionsId",
                table: "treatment_session_therapists",
                column: "TreatmentSessionsId");

            migrationBuilder.CreateIndex(
                name: "IX_treatment_sessions_treatment_id",
                table: "treatment_sessions",
                column: "treatment_id");

            migrationBuilder.CreateIndex(
                name: "IX_treatments_customer_id",
                table: "treatments",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_treatments_user_id",
                table: "treatments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_refresh_tokens_user_id",
                table: "user_refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_user_id",
                table: "user_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UX__users__username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "announcements");

            migrationBuilder.DropTable(
                name: "expense_photos");

            migrationBuilder.DropTable(
                name: "order_items");

            migrationBuilder.DropTable(
                name: "order_payments");

            migrationBuilder.DropTable(
                name: "order_photos");

            migrationBuilder.DropTable(
                name: "product_photos");

            migrationBuilder.DropTable(
                name: "role_claims");

            migrationBuilder.DropTable(
                name: "supply_photos");

            migrationBuilder.DropTable(
                name: "treatment_items");

            migrationBuilder.DropTable(
                name: "treatment_payments");

            migrationBuilder.DropTable(
                name: "treatment_photos");

            migrationBuilder.DropTable(
                name: "treatment_session_therapists");

            migrationBuilder.DropTable(
                name: "user_claims");

            migrationBuilder.DropTable(
                name: "user_logins");

            migrationBuilder.DropTable(
                name: "user_refresh_tokens");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "user_tokens");

            migrationBuilder.DropTable(
                name: "expenses");

            migrationBuilder.DropTable(
                name: "supply_items");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "treatment_sessions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "expense_categories");

            migrationBuilder.DropTable(
                name: "expenses_payees");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "supplies");

            migrationBuilder.DropTable(
                name: "treatments");

            migrationBuilder.DropTable(
                name: "brands");

            migrationBuilder.DropTable(
                name: "product_categories");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "countries");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
