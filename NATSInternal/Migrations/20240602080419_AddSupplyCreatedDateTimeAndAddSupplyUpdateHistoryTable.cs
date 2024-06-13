using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NATSInternal.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplyCreatedDateTimeAndAddSupplyUpdateHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_datetime",
                table: "supplies",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "supply_update_histories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    updated_datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    reason = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    old_data = table.Column<string>(type: "JSON", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    new_data = table.Column<string>(type: "JSON", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    supply_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supply_update_histories", x => x.id);
                    table.ForeignKey(
                        name: "FK__supply_update_histories__supplies__supply_id",
                        column: x => x.supply_id,
                        principalTable: "supplies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_supply_update_histories_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_supply_update_histories_supply_id",
                table: "supply_update_histories",
                column: "supply_id");

            migrationBuilder.CreateIndex(
                name: "IX_supply_update_histories_user_id",
                table: "supply_update_histories",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "supply_update_histories");

            migrationBuilder.DropColumn(
                name: "created_datetime",
                table: "supplies");
        }
    }
}
