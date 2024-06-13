using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NATSInternal.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForSuppliesSuppliedDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UX__supply_supplied_datetime",
                table: "supplies",
                column: "supplied_datetime",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX__supply_supplied_datetime",
                table: "supplies");
        }
    }
}
