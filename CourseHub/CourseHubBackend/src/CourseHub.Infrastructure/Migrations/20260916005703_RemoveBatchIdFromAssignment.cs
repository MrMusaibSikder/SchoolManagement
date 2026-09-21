using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBatchIdFromAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Batches_BatchId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_BatchId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentSubmissions_Batches_BatchId",
                table: "AssignmentSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentSubmissions_BatchId",
                table: "AssignmentSubmissions");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "AssignmentSubmissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BatchId",
                table: "Assignments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_BatchId",
                table: "Assignments",
                column: "BatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Batches_BatchId",
                table: "Assignments",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddColumn<Guid>(
                name: "BatchId",
                table: "AssignmentSubmissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_BatchId",
                table: "AssignmentSubmissions",
                column: "BatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSubmissions_Batches_BatchId",
                table: "AssignmentSubmissions",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
