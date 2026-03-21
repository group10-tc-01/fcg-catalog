using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Catalog.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class addaudittrail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "PurchaseTransactions",
                schema: "Catalog",
                newName: "PurchaseTransactions");

            migrationBuilder.RenameTable(
                name: "Promotions",
                schema: "Catalog",
                newName: "Promotions");

            migrationBuilder.RenameTable(
                name: "LibraryGames",
                schema: "Catalog",
                newName: "LibraryGames");

            migrationBuilder.RenameTable(
                name: "Libraries",
                schema: "Catalog",
                newName: "Libraries");

            migrationBuilder.RenameTable(
                name: "Games",
                schema: "Catalog",
                newName: "Games");

            migrationBuilder.CreateTable(
                name: "AuditTrail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerformedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PrimaryKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrailType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedColumns = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrail", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrail_EntityName",
                table: "AuditTrail",
                column: "EntityName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrail");

            migrationBuilder.EnsureSchema(
                name: "Catalog");

            migrationBuilder.RenameTable(
                name: "PurchaseTransactions",
                newName: "PurchaseTransactions",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "Promotions",
                newName: "Promotions",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "LibraryGames",
                newName: "LibraryGames",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "Libraries",
                newName: "Libraries",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "Games",
                newName: "Games",
                newSchema: "Catalog");
        }
    }
}
