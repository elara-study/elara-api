using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentParentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InitiatedById",
                table: "StudentParents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "StudentParents",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitiatedById",
                table: "StudentParents");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "StudentParents");
        }
    }
}
