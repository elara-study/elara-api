using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicIdToRoadmapAndModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PublicId",
                table: "ModuleResources",
                newName: "CloudId");

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Roadmaps",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Modules",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.CreateIndex(
                name: "IX_Roadmaps_PublicId",
                table: "Roadmaps",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modules_PublicId",
                table: "Modules",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Roadmaps_PublicId",
                table: "Roadmaps");

            migrationBuilder.DropIndex(
                name: "IX_Modules_PublicId",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Roadmaps");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Modules");

            migrationBuilder.RenameColumn(
                name: "CloudId",
                table: "ModuleResources",
                newName: "PublicId");
        }
    }
}
