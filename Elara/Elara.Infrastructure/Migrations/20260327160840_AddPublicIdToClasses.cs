using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicIdToClasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pgcrypto;");

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Classes",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_PublicId",
                table: "Classes",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Classes_PublicId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Classes");
        }
    }
}
