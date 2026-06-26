using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CollapseElaraInsight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsightRecommendation",
                table: "QuizSessions");

            migrationBuilder.DropColumn(
                name: "WeakTopics",
                table: "QuizSessions");

            migrationBuilder.AlterColumn<string>(
                name: "ElaraInsight",
                table: "QuizSessions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ElaraInsight",
                table: "QuizSessions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InsightRecommendation",
                table: "QuizSessions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeakTopics",
                table: "QuizSessions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
