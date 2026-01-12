using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Catalog.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class createtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Catalog");

            migrationBuilder.CreateTable(
                name: "Games",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.CheckConstraint("CK_Game_Price", "Price > 0");
                });

            migrationBuilder.CreateTable(
                name: "Libraries",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libraries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseTransactions",
                schema: "Catalog",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseTransactions", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                    table.CheckConstraint("CK_Promotion_Dates", "EndDate > StartDate");
                    table.CheckConstraint("CK_Promotion_Discount", "DiscountPercentage > 0 AND DiscountPercentage <= 100");
                    table.ForeignKey(
                        name: "FK_Promotions_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "Catalog",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LibraryGames",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GameId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryGames", x => x.Id);
                    table.CheckConstraint("CK_LibraryGame_Price", "PurchasePrice >= 0");
                    table.ForeignKey(
                        name: "FK_LibraryGames_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "Catalog",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LibraryGames_Games_GameId1",
                        column: x => x.GameId1,
                        principalSchema: "Catalog",
                        principalTable: "Games",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LibraryGames_Libraries_LibraryId",
                        column: x => x.LibraryId,
                        principalSchema: "Catalog",
                        principalTable: "Libraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_Category",
                schema: "Catalog",
                table: "Games",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Games_IsActive",
                schema: "Catalog",
                table: "Games",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Libraries_UserId",
                schema: "Catalog",
                table: "Libraries",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LibraryGames_GameId",
                schema: "Catalog",
                table: "LibraryGames",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryGames_GameId1",
                schema: "Catalog",
                table: "LibraryGames",
                column: "GameId1");

            migrationBuilder.CreateIndex(
                name: "UQ_LibraryGames_LibraryId_GameId",
                schema: "Catalog",
                table: "LibraryGames",
                columns: new[] { "LibraryId", "GameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_Dates",
                schema: "Catalog",
                table: "Promotions",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_GameId",
                schema: "Catalog",
                table: "Promotions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTransactions_GameId",
                schema: "Catalog",
                table: "PurchaseTransactions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTransactions_Status",
                schema: "Catalog",
                table: "PurchaseTransactions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTransactions_UserId",
                schema: "Catalog",
                table: "PurchaseTransactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LibraryGames",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "Promotions",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "PurchaseTransactions",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "Libraries",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "Catalog");
        }
    }
}
