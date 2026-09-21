using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseAssignedTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTeacherId",
                table: "Courses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_AssignedTeacherId",
                table: "Courses",
                column: "AssignedTeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Teachers_AssignedTeacherId",
                table: "Courses",
                column: "AssignedTeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Teachers_AssignedTeacherId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_AssignedTeacherId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "AssignedTeacherId",
                table: "Courses");
        }
    }
}
