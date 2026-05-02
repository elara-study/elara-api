using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addquizsystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentStreak",
                table: "Students",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityDate",
                table: "Students",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Students",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "TotalXP",
                table: "Students",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "InProgress"),
                    XpEarned = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CorrectAnswers = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    WrongAnswers = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    UnansweredCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ElaraInsight = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    WeakTopics = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    InsightRecommendation = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizSessions_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizSessions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionType = table.Column<string>(type: "text", nullable: false),
                    SelectedOptionId = table.Column<int>(type: "integer", nullable: true),
                    AnswerContent = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: true),
                    XpAwarded = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    HintUsed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    QuizSessionId = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAnswers_QuestionOptions_SelectedOptionId",
                        column: x => x.SelectedOptionId,
                        principalTable: "QuestionOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizAnswers_QuizSessions_QuizSessionId",
                        column: x => x.QuizSessionId,
                        principalTable: "QuizSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_TotalXP",
                table: "Students",
                column: "TotalXP");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuestionId",
                table: "QuestionOptions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_QuestionId",
                table: "QuizAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_QuizSessionId",
                table: "QuizAnswers",
                column: "QuizSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_SelectedOptionId",
                table: "QuizAnswers",
                column: "SelectedOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSessions_AssignmentId",
                table: "QuizSessions",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSessions_StudentId",
                table: "QuizSessions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSessions_StudentId_Status",
                table: "QuizSessions",
                columns: new[] { "StudentId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizAnswers");

            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.DropTable(
                name: "QuizSessions");

            migrationBuilder.DropIndex(
                name: "IX_Students_TotalXP",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CurrentStreak",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LastActivityDate",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "TotalXP",
                table: "Students");
        }
    }
}
