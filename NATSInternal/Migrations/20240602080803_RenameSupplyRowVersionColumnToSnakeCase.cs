using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NATSInternal.Migrations
{
    /// <inheritdoc />
    public partial class RenameSupplyRowVersionColumnToSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RowVersion",
                table: "supplies",
                newName: "row_version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "row_version",
                table: "supplies",
                newName: "RowVersion");
        }
    }
}
