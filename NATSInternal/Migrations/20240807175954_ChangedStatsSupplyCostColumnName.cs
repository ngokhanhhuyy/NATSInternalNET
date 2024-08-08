using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NATSInternal.Migrations
{
    /// <inheritdoc />
    public partial class ChangedStatsSupplyCostColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "supply_expense",
                table: "monthly_stats",
                newName: "supply_cost");

            migrationBuilder.RenameColumn(
                name: "supply_expense",
                table: "daily_stats",
                newName: "supply_cost");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "supply_cost",
                table: "monthly_stats",
                newName: "supply_expense");

            migrationBuilder.RenameColumn(
                name: "supply_cost",
                table: "daily_stats",
                newName: "supply_expense");
        }
    }
}
