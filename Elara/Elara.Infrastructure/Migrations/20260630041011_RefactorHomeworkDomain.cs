using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorHomeworkDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hints_Questions_QuestionId",
                table: "Hints");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizSessions_ProblemSets_ProblemSetId",
                table: "QuizSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubmissionAnswers_Questions_QuestionId",
                table: "StudentSubmissionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubmissions_ProblemSets_ProblemSetId",
                table: "StudentSubmissions");

            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "ProblemSets");

            migrationBuilder.RenameColumn(
                name: "ProblemSetId",
                table: "StudentSubmissions",
                newName: "HomeworkId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubmissions_StudentId_ProblemSetId",
                table: "StudentSubmissions",
                newName: "IX_StudentSubmissions_StudentId_HomeworkId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubmissions_ProblemSetId",
                table: "StudentSubmissions",
                newName: "IX_StudentSubmissions_HomeworkId");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "StudentSubmissionAnswers",
                newName: "ProblemId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubmissionAnswers_QuestionId",
                table: "StudentSubmissionAnswers",
                newName: "IX_StudentSubmissionAnswers_ProblemId");

            migrationBuilder.RenameColumn(
                name: "ProblemSetId",
                table: "QuizSessions",
                newName: "HomeworkId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizSessions_ProblemSetId",
                table: "QuizSessions",
                newName: "IX_QuizSessions_HomeworkId");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "Hints",
                newName: "ProblemId");

            migrationBuilder.RenameIndex(
                name: "IX_Hints_QuestionId_StudentId",
                table: "Hints",
                newName: "IX_Hints_ProblemId_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Hints_QuestionId",
                table: "Hints",
                newName: "IX_Hints_ProblemId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Homework",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DifficultyLevel",
                table: "Homework",
                type: "text",
                nullable: false,
                defaultValue: "Easy");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Homework",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsAIGenerated",
                table: "Homework",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "Homework",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxScore",
                table: "Homework",
                type: "integer",
                nullable: false,
                defaultValue: 100);

            migrationBuilder.CreateTable(
                name: "Problems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    QuestionType = table.Column<string>(type: "text", nullable: false),
                    DifficultyLevel = table.Column<string>(type: "text", nullable: false, defaultValue: "Easy"),
                    IsAIGenerated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasVideoSupport = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Marks = table.Column<double>(type: "double precision", precision: 5, scale: 2, nullable: false, defaultValue: 10.0),
                    HomeworkId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Problems_Homework_HomeworkId",
                        column: x => x.HomeworkId,
                        principalTable: "Homework",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProblemOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ProblemId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProblemOptions_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProblemOptions_ProblemId",
                table: "ProblemOptions",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_DifficultyLevel",
                table: "Problems",
                column: "DifficultyLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_HomeworkId",
                table: "Problems",
                column: "HomeworkId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_QuestionType",
                table: "Problems",
                column: "QuestionType");

            migrationBuilder.AddForeignKey(
                name: "FK_Hints_Problems_ProblemId",
                table: "Hints",
                column: "ProblemId",
                principalTable: "Problems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizSessions_Homework_HomeworkId",
                table: "QuizSessions",
                column: "HomeworkId",
                principalTable: "Homework",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubmissionAnswers_Problems_ProblemId",
                table: "StudentSubmissionAnswers",
                column: "ProblemId",
                principalTable: "Problems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubmissions_Homework_HomeworkId",
                table: "StudentSubmissions",
                column: "HomeworkId",
                principalTable: "Homework",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hints_Problems_ProblemId",
                table: "Hints");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizSessions_Homework_HomeworkId",
                table: "QuizSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubmissionAnswers_Problems_ProblemId",
                table: "StudentSubmissionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubmissions_Homework_HomeworkId",
                table: "StudentSubmissions");

            migrationBuilder.DropTable(
                name: "ProblemOptions");

            migrationBuilder.DropTable(
                name: "Problems");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Homework");

            migrationBuilder.DropColumn(
                name: "DifficultyLevel",
                table: "Homework");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Homework");

            migrationBuilder.DropColumn(
                name: "IsAIGenerated",
                table: "Homework");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "Homework");

            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "Homework");

            migrationBuilder.RenameColumn(
                name: "HomeworkId",
                table: "StudentSubmissions",
                newName: "ProblemSetId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubmissions_StudentId_HomeworkId",
                table: "StudentSubmissions",
                newName: "IX_StudentSubmissions_StudentId_ProblemSetId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubmissions_HomeworkId",
                table: "StudentSubmissions",
                newName: "IX_StudentSubmissions_ProblemSetId");

            migrationBuilder.RenameColumn(
                name: "ProblemId",
                table: "StudentSubmissionAnswers",
                newName: "QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubmissionAnswers_ProblemId",
                table: "StudentSubmissionAnswers",
                newName: "IX_StudentSubmissionAnswers_QuestionId");

            migrationBuilder.RenameColumn(
                name: "HomeworkId",
                table: "QuizSessions",
                newName: "ProblemSetId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizSessions_HomeworkId",
                table: "QuizSessions",
                newName: "IX_QuizSessions_ProblemSetId");

            migrationBuilder.RenameColumn(
                name: "ProblemId",
                table: "Hints",
                newName: "QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Hints_ProblemId_StudentId",
                table: "Hints",
                newName: "IX_Hints_QuestionId_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Hints_ProblemId",
                table: "Hints",
                newName: "IX_Hints_QuestionId");

            migrationBuilder.CreateTable(
                name: "ProblemSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HomeworkId = table.Column<int>(type: "integer", nullable: true),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DifficultyLevel = table.Column<string>(type: "text", nullable: false, defaultValue: "Easy"),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAIGenerated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    MaxScore = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    ProblemSetType = table.Column<string>(type: "text", nullable: false, defaultValue: "Quiz"),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProblemSets_Homework_HomeworkId",
                        column: x => x.HomeworkId,
                        principalTable: "Homework",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProblemSets_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProblemSets_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProblemSetId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DifficultyLevel = table.Column<string>(type: "text", nullable: false, defaultValue: "Easy"),
                    HasVideoSupport = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsAIGenerated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Marks = table.Column<double>(type: "double precision", precision: 5, scale: 2, nullable: false, defaultValue: 10.0),
                    QuestionType = table.Column<string>(type: "text", nullable: false),
                    Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_ProblemSets_ProblemSetId",
                        column: x => x.ProblemSetId,
                        principalTable: "ProblemSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_ProblemSets_DueDate",
                table: "ProblemSets",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemSets_HomeworkId",
                table: "ProblemSets",
                column: "HomeworkId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemSets_ModuleId",
                table: "ProblemSets",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemSets_TeacherId",
                table: "ProblemSets",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuestionId",
                table: "QuestionOptions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_DifficultyLevel",
                table: "Questions",
                column: "DifficultyLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ProblemSetId",
                table: "Questions",
                column: "ProblemSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionType",
                table: "Questions",
                column: "QuestionType");

            migrationBuilder.AddForeignKey(
                name: "FK_Hints_Questions_QuestionId",
                table: "Hints",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizSessions_ProblemSets_ProblemSetId",
                table: "QuizSessions",
                column: "ProblemSetId",
                principalTable: "ProblemSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubmissionAnswers_Questions_QuestionId",
                table: "StudentSubmissionAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubmissions_ProblemSets_ProblemSetId",
                table: "StudentSubmissions",
                column: "ProblemSetId",
                principalTable: "ProblemSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
