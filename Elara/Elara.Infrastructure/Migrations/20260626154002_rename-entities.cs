using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class renameentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Assignments_AssignmentId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswers_QuestionOptions_SelectedOptionId",
                table: "QuizAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswers_Questions_QuestionId",
                table: "QuizAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizSessions_Assignments_AssignmentId",
                table: "QuizSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubmissions_Assignments_AssignmentId",
                table: "StudentSubmissions");

            // Clear existing data to avoid FK conflicts during schema migration
            migrationBuilder.Sql("DELETE FROM \"QuizAnswers\"");
            migrationBuilder.Sql("DELETE FROM \"QuizSessions\"");
            migrationBuilder.Sql("DELETE FROM \"StudentSubmissions\"");
            migrationBuilder.Sql("DELETE FROM \"Questions\"");
            migrationBuilder.Sql("DELETE FROM \"Roadmaps\"");
            migrationBuilder.Sql("DELETE FROM \"Subjects\"");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "LessonVideos");

            migrationBuilder.DropTable(
                name: "TopicResources");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_QuizSessions_AssignmentId",
                table: "QuizSessions");

            migrationBuilder.DropIndex(
                name: "IX_QuizAnswers_QuestionId",
                table: "QuizAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuizAnswers_SelectedOptionId",
                table: "QuizAnswers");

            migrationBuilder.DropColumn(
                name: "AssignmentId",
                table: "QuizSessions");

            migrationBuilder.DropColumn(
                name: "AnswerContent",
                table: "QuizAnswers");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "QuizAnswers");

            migrationBuilder.DropColumn(
                name: "SelectedOptionId",
                table: "QuizAnswers");

            migrationBuilder.RenameColumn(
                name: "AssignmentId",
                table: "StudentSubmissions",
                newName: "ProblemSetId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubmissions_StudentId_AssignmentId",
                table: "StudentSubmissions",
                newName: "IX_StudentSubmissions_StudentId_ProblemSetId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubmissions_AssignmentId",
                table: "StudentSubmissions",
                newName: "IX_StudentSubmissions_ProblemSetId");

            migrationBuilder.RenameColumn(
                name: "AssignmentId",
                table: "Questions",
                newName: "ProblemSetId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_AssignmentId",
                table: "Questions",
                newName: "IX_Questions_ProblemSetId");

            migrationBuilder.AddColumn<int>(
                name: "ModuleId",
                table: "QuizSessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProblemSetId",
                table: "QuizSessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionsJson",
                table: "QuizSessions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrectAnswer",
                table: "QuizAnswers",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QuestionText",
                table: "QuizAnswers",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentAnswer",
                table: "QuizAnswers",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    ModuleName = table.Column<string>(type: "text", nullable: true),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    RoadmapId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modules_Roadmaps_RoadmapId",
                        column: x => x.RoadmapId,
                        principalTable: "Roadmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Modules_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Homework",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    EstimatedDurationMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 30),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Homework", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Homework_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModuleResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    PublicId = table.Column<string>(type: "text", nullable: true),
                    ResourceType = table.Column<int>(type: "integer", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: true),
                    SizeOrDurationText = table.Column<string>(type: "text", nullable: true),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleResources_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeworkVideos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HomeworkId = table.Column<int>(type: "integer", nullable: false),
                    VideoId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeworkVideos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeworkVideos_EducationalVideos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "EducationalVideos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HomeworkVideos_Homework_HomeworkId",
                        column: x => x.HomeworkId,
                        principalTable: "Homework",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProblemSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaxScore = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsAIGenerated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DifficultyLevel = table.Column<string>(type: "text", nullable: false, defaultValue: "Easy"),
                    ProblemSetType = table.Column<string>(type: "text", nullable: false, defaultValue: "Quiz"),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    HomeworkId = table.Column<int>(type: "integer", nullable: true),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_QuizSessions_ModuleId",
                table: "QuizSessions",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSessions_ProblemSetId",
                table: "QuizSessions",
                column: "ProblemSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Homework_ModuleId",
                table: "Homework",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkVideos_HomeworkId",
                table: "HomeworkVideos",
                column: "HomeworkId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkVideos_HomeworkId_VideoId",
                table: "HomeworkVideos",
                columns: new[] { "HomeworkId", "VideoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkVideos_VideoId",
                table: "HomeworkVideos",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleResources_ModuleId",
                table: "ModuleResources",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_RoadmapId",
                table: "Modules",
                column: "RoadmapId");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_SubjectId",
                table: "Modules",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_Title",
                table: "Modules",
                column: "Title");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_ProblemSets_ProblemSetId",
                table: "Questions",
                column: "ProblemSetId",
                principalTable: "ProblemSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizSessions_Modules_ModuleId",
                table: "QuizSessions",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizSessions_ProblemSets_ProblemSetId",
                table: "QuizSessions",
                column: "ProblemSetId",
                principalTable: "ProblemSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubmissions_ProblemSets_ProblemSetId",
                table: "StudentSubmissions",
                column: "ProblemSetId",
                principalTable: "ProblemSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_ProblemSets_ProblemSetId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizSessions_Modules_ModuleId",
                table: "QuizSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizSessions_ProblemSets_ProblemSetId",
                table: "QuizSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubmissions_ProblemSets_ProblemSetId",
                table: "StudentSubmissions");

            migrationBuilder.DropTable(
                name: "HomeworkVideos");

            migrationBuilder.DropTable(
                name: "ModuleResources");

            migrationBuilder.DropTable(
                name: "ProblemSets");

            migrationBuilder.DropTable(
                name: "Homework");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropIndex(
                name: "IX_QuizSessions_ModuleId",
                table: "QuizSessions");

            migrationBuilder.DropIndex(
                name: "IX_QuizSessions_ProblemSetId",
                table: "QuizSessions");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "QuizSessions");

            migrationBuilder.DropColumn(
                name: "ProblemSetId",
                table: "QuizSessions");

            migrationBuilder.DropColumn(
                name: "QuestionsJson",
                table: "QuizSessions");

            migrationBuilder.DropColumn(
                name: "CorrectAnswer",
                table: "QuizAnswers");

            migrationBuilder.DropColumn(
                name: "QuestionText",
                table: "QuizAnswers");

            migrationBuilder.DropColumn(
                name: "StudentAnswer",
                table: "QuizAnswers");

            migrationBuilder.RenameColumn(
                name: "ProblemSetId",
                table: "StudentSubmissions",
                newName: "AssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubmissions_StudentId_ProblemSetId",
                table: "StudentSubmissions",
                newName: "IX_StudentSubmissions_StudentId_AssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubmissions_ProblemSetId",
                table: "StudentSubmissions",
                newName: "IX_StudentSubmissions_AssignmentId");

            migrationBuilder.RenameColumn(
                name: "ProblemSetId",
                table: "Questions",
                newName: "AssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_ProblemSetId",
                table: "Questions",
                newName: "IX_Questions_AssignmentId");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentId",
                table: "QuizSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AnswerContent",
                table: "QuizAnswers",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "QuizAnswers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SelectedOptionId",
                table: "QuizAnswers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoadmapId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModuleName = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topics_Roadmaps_RoadmapId",
                        column: x => x.RoadmapId,
                        principalTable: "Roadmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Topics_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TopicId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EstimatedDurationMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 30),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TopicResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TopicId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    PublicId = table.Column<string>(type: "text", nullable: true),
                    ResourceType = table.Column<int>(type: "integer", nullable: false),
                    SizeOrDurationText = table.Column<string>(type: "text", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicResources_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LessonId = table.Column<int>(type: "integer", nullable: true),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: true),
                    TopicId = table.Column<int>(type: "integer", nullable: false),
                    AssignmentType = table.Column<string>(type: "text", nullable: false, defaultValue: "Quiz"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DifficultyLevel = table.Column<string>(type: "text", nullable: false, defaultValue: "Easy"),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAIGenerated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    MaxScore = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Assignments_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Assignments_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LessonVideos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LessonId = table.Column<int>(type: "integer", nullable: false),
                    VideoId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonVideos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonVideos_EducationalVideos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "EducationalVideos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonVideos_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizSessions_AssignmentId",
                table: "QuizSessions",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_QuestionId",
                table: "QuizAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_SelectedOptionId",
                table: "QuizAnswers",
                column: "SelectedOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_DueDate",
                table: "Assignments",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_LessonId",
                table: "Assignments",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TeacherId",
                table: "Assignments",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TopicId",
                table: "Assignments",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_TopicId",
                table: "Lessons",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonVideos_LessonId",
                table: "LessonVideos",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonVideos_LessonId_VideoId",
                table: "LessonVideos",
                columns: new[] { "LessonId", "VideoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonVideos_VideoId",
                table: "LessonVideos",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicResources_TopicId",
                table: "TopicResources",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_RoadmapId",
                table: "Topics",
                column: "RoadmapId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_SubjectId",
                table: "Topics",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Title",
                table: "Topics",
                column: "Title");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Assignments_AssignmentId",
                table: "Questions",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswers_QuestionOptions_SelectedOptionId",
                table: "QuizAnswers",
                column: "SelectedOptionId",
                principalTable: "QuestionOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswers_Questions_QuestionId",
                table: "QuizAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizSessions_Assignments_AssignmentId",
                table: "QuizSessions",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubmissions_Assignments_AssignmentId",
                table: "StudentSubmissions",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
