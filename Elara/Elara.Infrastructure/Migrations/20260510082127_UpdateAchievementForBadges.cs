using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAchievementForBadges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EarnedAt",
                table: "Achievements");

            migrationBuilder.RenameColumn(
                name: "IconPath",
                table: "Achievements",
                newName: "ImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "ImagePublicId",
                table: "Achievements",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TargetValue",
                table: "Achievements",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "TargetValue",
                table: "Achievements");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Achievements",
                newName: "IconPath");

            migrationBuilder.AddColumn<DateTime>(
                name: "EarnedAt",
                table: "Achievements",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
