using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elara.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultSubjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO ""Subjects"" (""Id"", ""CreatedAt"", ""DeletedAt"", ""Description"", ""GradeLevel"", ""IsDeleted"", ""Name"", ""UpdatedAt"")
VALUES (1, TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, 'Chemistry', 'Grade12', FALSE, 'Chemistry', NULL)
ON CONFLICT (""Id"") DO UPDATE
SET ""Name"" = EXCLUDED.""Name"",
    ""Description"" = EXCLUDED.""Description"",
    ""GradeLevel"" = EXCLUDED.""GradeLevel"",
    ""IsDeleted"" = FALSE,
    ""DeletedAt"" = NULL;

INSERT INTO ""Subjects"" (""Id"", ""CreatedAt"", ""DeletedAt"", ""Description"", ""GradeLevel"", ""IsDeleted"", ""Name"", ""UpdatedAt"")
VALUES (2, TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, 'Physics', 'Grade12', FALSE, 'Physics', NULL)
ON CONFLICT (""Id"") DO UPDATE
SET ""Name"" = EXCLUDED.""Name"",
    ""Description"" = EXCLUDED.""Description"",
    ""GradeLevel"" = EXCLUDED.""GradeLevel"",
    ""IsDeleted"" = FALSE,
    ""DeletedAt"" = NULL;

INSERT INTO ""Subjects"" (""Id"", ""CreatedAt"", ""DeletedAt"", ""Description"", ""GradeLevel"", ""IsDeleted"", ""Name"", ""UpdatedAt"")
VALUES (3, TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, 'Biology', 'Grade12', FALSE, 'Biology', NULL)
ON CONFLICT (""Id"") DO UPDATE
SET ""Name"" = EXCLUDED.""Name"",
    ""Description"" = EXCLUDED.""Description"",
    ""GradeLevel"" = EXCLUDED.""GradeLevel"",
    ""IsDeleted"" = FALSE,
    ""DeletedAt"" = NULL;

INSERT INTO ""Subjects"" (""Id"", ""CreatedAt"", ""DeletedAt"", ""Description"", ""GradeLevel"", ""IsDeleted"", ""Name"", ""UpdatedAt"")
VALUES (4, TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, 'PureMathematics', 'Grade12', FALSE, 'PureMathematics', NULL)
ON CONFLICT (""Id"") DO UPDATE
SET ""Name"" = EXCLUDED.""Name"",
    ""Description"" = EXCLUDED.""Description"",
    ""GradeLevel"" = EXCLUDED.""GradeLevel"",
    ""IsDeleted"" = FALSE,
    ""DeletedAt"" = NULL;

INSERT INTO ""Subjects"" (""Id"", ""CreatedAt"", ""DeletedAt"", ""Description"", ""GradeLevel"", ""IsDeleted"", ""Name"", ""UpdatedAt"")
VALUES (5, TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, 'AppliedMathematics', 'Grade12', FALSE, 'AppliedMathematics', NULL)
ON CONFLICT (""Id"") DO UPDATE
SET ""Name"" = EXCLUDED.""Name"",
    ""Description"" = EXCLUDED.""Description"",
    ""GradeLevel"" = EXCLUDED.""GradeLevel"",
    ""IsDeleted"" = FALSE,
    ""DeletedAt"" = NULL;

INSERT INTO ""Subjects"" (""Id"", ""CreatedAt"", ""DeletedAt"", ""Description"", ""GradeLevel"", ""IsDeleted"", ""Name"", ""UpdatedAt"")
VALUES (6, TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, 'Arabic', 'Grade12', FALSE, 'Arabic', NULL)
ON CONFLICT (""Id"") DO UPDATE
SET ""Name"" = EXCLUDED.""Name"",
    ""Description"" = EXCLUDED.""Description"",
    ""GradeLevel"" = EXCLUDED.""GradeLevel"",
    ""IsDeleted"" = FALSE,
    ""DeletedAt"" = NULL;

INSERT INTO ""Subjects"" (""Id"", ""CreatedAt"", ""DeletedAt"", ""Description"", ""GradeLevel"", ""IsDeleted"", ""Name"", ""UpdatedAt"")
VALUES (7, TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, 'English', 'Grade12', FALSE, 'English', NULL)
ON CONFLICT (""Id"") DO UPDATE
SET ""Name"" = EXCLUDED.""Name"",
    ""Description"" = EXCLUDED.""Description"",
    ""GradeLevel"" = EXCLUDED.""GradeLevel"",
    ""IsDeleted"" = FALSE,
    ""DeletedAt"" = NULL;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
