using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonavixxHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class FlashcardSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlashcardSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    ParentSetId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlashcardSets_FlashcardSets_ParentSetId",
                        column: x => x.ParentSetId,
                        principalTable: "FlashcardSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlashcardSets_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlashcardSetEntries",
                columns: table => new
                {
                    FlashcardSetId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FlashcardId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardSetEntries", x => new { x.FlashcardId, x.FlashcardSetId });
                    table.ForeignKey(
                        name: "FK_FlashcardSetEntries_FlashcardSets_FlashcardSetId",
                        column: x => x.FlashcardSetId,
                        principalTable: "FlashcardSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlashcardSetEntries_Flashcards_FlashcardId",
                        column: x => x.FlashcardId,
                        principalTable: "Flashcards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlashcardSetEntries_FlashcardSetId",
                table: "FlashcardSetEntries",
                column: "FlashcardSetId");

            migrationBuilder.CreateIndex(
                name: "IX_FlashcardSets_OwnerId",
                table: "FlashcardSets",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FlashcardSets_ParentSetId",
                table: "FlashcardSets",
                column: "ParentSetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlashcardSetEntries");

            migrationBuilder.DropTable(
                name: "FlashcardSets");
        }
    }
}
