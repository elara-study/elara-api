using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeRoadmapNameUniquePerTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Roadmaps_TeacherId_Name",
                table: "Roadmaps");

            migrationBuilder.CreateIndex(
                name: "IX_Roadmaps_TeacherId_Name",
                table: "Roadmaps",
                columns: new[] { "TeacherId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Roadmaps_TeacherId_Name",
                table: "Roadmaps");

            migrationBuilder.CreateIndex(
                name: "IX_Roadmaps_TeacherId_Name",
                table: "Roadmaps",
                columns: new[] { "TeacherId", "Name" });
        }
    }
}
