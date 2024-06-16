using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NATSInternal.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesAndColumnsForStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "paid_amount",
                table: "supplies");

            migrationBuilder.DropColumn(
                name: "shipment_fee",
                table: "orders");

            migrationBuilder.RenameColumn(
                name: "shipment_fee_included",
                table: "orders",
                newName: "is_closed");

            migrationBuilder.AddColumn<bool>(
                name: "is_closed",
                table: "treatments",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_closed",
                table: "treatment_sessions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_closed",
                table: "treatment_payments",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_closed",
                table: "order_payments",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "billing_month",
                table: "expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "billing_year",
                table: "expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "is_closed",
                table: "expenses",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "expense_categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "monthly_stats",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    retail_revenue = table.Column<long>(type: "bigint", nullable: false),
                    treatment_revenue = table.Column<long>(type: "bigint", nullable: false),
                    consultant_revenue = table.Column<long>(type: "bigint", nullable: false),
                    shipment_cost = table.Column<long>(type: "bigint", nullable: false),
                    supply_expense = table.Column<long>(type: "bigint", nullable: false),
                    utilities_expenses = table.Column<long>(type: "bigint", nullable: false),
                    equipment_expenses = table.Column<long>(type: "bigint", nullable: false),
                    office_expese = table.Column<long>(type: "bigint", nullable: false),
                    staff_expense = table.Column<long>(type: "bigint", nullable: false),
                    recorded_month = table.Column<long>(type: "bigint", nullable: false),
                    recoreded_year = table.Column<long>(type: "bigint", nullable: false),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    temporarily_closed_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    officially_closed_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    monthly_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monthly_stats", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "daily_stats",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    retail_revenue = table.Column<long>(type: "bigint", nullable: false),
                    treatment_revenue = table.Column<long>(type: "bigint", nullable: false),
                    consultant_revenue = table.Column<long>(type: "bigint", nullable: false),
                    shipment_cost = table.Column<long>(type: "bigint", nullable: false),
                    supply_expense = table.Column<long>(type: "bigint", nullable: false),
                    utilities_expenses = table.Column<long>(type: "bigint", nullable: false),
                    equipment_expenses = table.Column<long>(type: "bigint", nullable: false),
                    office_expese = table.Column<long>(type: "bigint", nullable: false),
                    staff_expense = table.Column<long>(type: "bigint", nullable: false),
                    recorded_date = table.Column<DateOnly>(type: "date", nullable: false),
                    created_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    temporarily_closed_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    officially_closed_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    monthly_stats_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_stats", x => x.id);
                    table.ForeignKey(
                        name: "FK__daily_stats__monthly_stats__monthly_id",
                        column: x => x.monthly_stats_id,
                        principalTable: "monthly_stats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX__paid_datetime__billing_month__billing_year",
                table: "expenses",
                columns: new[] { "paid_datetime", "billing_month", "billing_year" });

            migrationBuilder.CreateIndex(
                name: "IX_daily_stats_monthly_stats_id",
                table: "daily_stats",
                column: "monthly_stats_id");

            migrationBuilder.CreateIndex(
                name: "UX__daily_stats__recorded_date",
                table: "daily_stats",
                column: "recorded_date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_monthly_stats__recorded_month__recorded_year",
                table: "monthly_stats",
                columns: new[] { "recorded_month", "recoreded_year" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "daily_stats");

            migrationBuilder.DropTable(
                name: "monthly_stats");

            migrationBuilder.DropIndex(
                name: "IX__paid_datetime__billing_month__billing_year",
                table: "expenses");

            migrationBuilder.DropColumn(
                name: "is_closed",
                table: "treatments");

            migrationBuilder.DropColumn(
                name: "is_closed",
                table: "treatment_sessions");

            migrationBuilder.DropColumn(
                name: "is_closed",
                table: "treatment_payments");

            migrationBuilder.DropColumn(
                name: "is_closed",
                table: "order_payments");

            migrationBuilder.DropColumn(
                name: "billing_month",
                table: "expenses");

            migrationBuilder.DropColumn(
                name: "billing_year",
                table: "expenses");

            migrationBuilder.DropColumn(
                name: "is_closed",
                table: "expenses");

            migrationBuilder.DropColumn(
                name: "type",
                table: "expense_categories");

            migrationBuilder.RenameColumn(
                name: "is_closed",
                table: "orders",
                newName: "shipment_fee_included");

            migrationBuilder.AddColumn<long>(
                name: "paid_amount",
                table: "supplies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "shipment_fee",
                table: "orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
