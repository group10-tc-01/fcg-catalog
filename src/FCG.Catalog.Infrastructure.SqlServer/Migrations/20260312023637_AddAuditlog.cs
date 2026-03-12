using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Catalog.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditTrails",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    EntityPrimaryKey = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    CorrelationId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrails", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_CorrelationId",
                schema: "Catalog",
                table: "AuditTrails",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_EntityName_EntityPrimaryKey",
                schema: "Catalog",
                table: "AuditTrails",
                columns: new[] { "EntityName", "EntityPrimaryKey" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_OccurredAt",
                schema: "Catalog",
                table: "AuditTrails",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_UserId",
                schema: "Catalog",
                table: "AuditTrails",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrails",
                schema: "Catalog");
        }
    }
}
