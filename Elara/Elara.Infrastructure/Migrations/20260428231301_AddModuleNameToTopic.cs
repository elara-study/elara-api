using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModuleNameToTopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModuleName",
                table: "Topics",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAIGenerated",
                table: "Assignments",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModuleName",
                table: "Topics");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAIGenerated",
                table: "Assignments",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }
    }
}
