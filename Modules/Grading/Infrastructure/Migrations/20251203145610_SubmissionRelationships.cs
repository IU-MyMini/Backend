using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradingModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SubmissionRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submission_CourseParticipant_CourseParticipantId",
                table: "Submission");

            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Group_GroupId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_CourseParticipantId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_GroupId",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "CourseParticipantId",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Submission");

            migrationBuilder.RenameColumn(
                name: "SubmittedByUserId",
                table: "Submission",
                newName: "SubmittedByParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_SubmittedByGroupId",
                table: "Submission",
                column: "SubmittedByGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_SubmittedByParticipantId",
                table: "Submission",
                column: "SubmittedByParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_CourseParticipant_SubmittedByParticipantId",
                table: "Submission",
                column: "SubmittedByParticipantId",
                principalTable: "CourseParticipant",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_Group_SubmittedByGroupId",
                table: "Submission",
                column: "SubmittedByGroupId",
                principalTable: "Group",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submission_CourseParticipant_SubmittedByParticipantId",
                table: "Submission");

            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Group_SubmittedByGroupId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_SubmittedByGroupId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_SubmittedByParticipantId",
                table: "Submission");

            migrationBuilder.RenameColumn(
                name: "SubmittedByParticipantId",
                table: "Submission",
                newName: "SubmittedByUserId");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseParticipantId",
                table: "Submission",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "Submission",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submission_CourseParticipantId",
                table: "Submission",
                column: "CourseParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_GroupId",
                table: "Submission",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_CourseParticipant_CourseParticipantId",
                table: "Submission",
                column: "CourseParticipantId",
                principalTable: "CourseParticipant",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_Group_GroupId",
                table: "Submission",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id");
        }
    }
}
