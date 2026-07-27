using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CourseSections_SemesterId",
                table: "CourseSections");

            migrationBuilder.CreateIndex(
                name: "IX_Tuitions_Status",
                table: "Tuitions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SubjectName",
                table: "Subjects",
                column: "SubjectName");

            migrationBuilder.CreateIndex(
                name: "IX_Students_FullName",
                table: "Students",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Status",
                table: "Students",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Majors_MajorName",
                table: "Majors",
                column: "MajorName");

            migrationBuilder.CreateIndex(
                name: "IX_Lecturers_FullName",
                table: "Lecturers",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSections_SemesterId_Status",
                table: "CourseSections",
                columns: new[] { "SemesterId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tuitions_Status",
                table: "Tuitions");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_SubjectName",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Students_FullName",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_Status",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Majors_MajorName",
                table: "Majors");

            migrationBuilder.DropIndex(
                name: "IX_Lecturers_FullName",
                table: "Lecturers");

            migrationBuilder.DropIndex(
                name: "IX_CourseSections_SemesterId_Status",
                table: "CourseSections");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSections_SemesterId",
                table: "CourseSections",
                column: "SemesterId");
        }
    }
}
