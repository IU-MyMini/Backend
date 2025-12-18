using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradingModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PeerReviewUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeerReview_AssignmentComponent_ComponentId",
                table: "PeerReview");

            migrationBuilder.DropForeignKey(
                name: "FK_PeerReview_CourseParticipant_CourseParticipantId",
                table: "PeerReview");

            migrationBuilder.DropForeignKey(
                name: "FK_PeerReview_Group_GroupId",
                table: "PeerReview");

            migrationBuilder.DropForeignKey(
                name: "FK_PeerReview_Submission_SubmissionId",
                table: "PeerReview");

            migrationBuilder.DropIndex(
                name: "IX_PeerReview_CourseParticipantId",
                table: "PeerReview");

            migrationBuilder.DropIndex(
                name: "IX_PeerReview_GroupId",
                table: "PeerReview");

            migrationBuilder.DropColumn(
                name: "CourseParticipantId",
                table: "PeerReview");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "PeerReview");

            migrationBuilder.RenameColumn(
                name: "SubmissionId",
                table: "PeerReview",
                newName: "TargetGroupId");

            migrationBuilder.RenameColumn(
                name: "ComponentId",
                table: "PeerReview",
                newName: "TargetComponentId");

            migrationBuilder.RenameIndex(
                name: "IX_PeerReview_SubmissionId",
                table: "PeerReview",
                newName: "IX_PeerReview_TargetGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_PeerReview_ComponentId",
                table: "PeerReview",
                newName: "IX_PeerReview_TargetComponentId");

            migrationBuilder.AddColumn<Guid>(
                name: "SourceComponentId",
                table: "PeerReview",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SourceGroupId",
                table: "PeerReview",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PeerReview_SourceComponentId",
                table: "PeerReview",
                column: "SourceComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_PeerReview_SourceGroupId",
                table: "PeerReview",
                column: "SourceGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_PeerReview_AssignmentComponent_SourceComponentId",
                table: "PeerReview",
                column: "SourceComponentId",
                principalTable: "AssignmentComponent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PeerReview_AssignmentComponent_TargetComponentId",
                table: "PeerReview",
                column: "TargetComponentId",
                principalTable: "AssignmentComponent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PeerReview_Group_SourceGroupId",
                table: "PeerReview",
                column: "SourceGroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PeerReview_Group_TargetGroupId",
                table: "PeerReview",
                column: "TargetGroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeerReview_AssignmentComponent_SourceComponentId",
                table: "PeerReview");

            migrationBuilder.DropForeignKey(
                name: "FK_PeerReview_AssignmentComponent_TargetComponentId",
                table: "PeerReview");

            migrationBuilder.DropForeignKey(
                name: "FK_PeerReview_Group_SourceGroupId",
                table: "PeerReview");

            migrationBuilder.DropForeignKey(
                name: "FK_PeerReview_Group_TargetGroupId",
                table: "PeerReview");

            migrationBuilder.DropIndex(
                name: "IX_PeerReview_SourceComponentId",
                table: "PeerReview");

            migrationBuilder.DropIndex(
                name: "IX_PeerReview_SourceGroupId",
                table: "PeerReview");

            migrationBuilder.DropColumn(
                name: "SourceComponentId",
                table: "PeerReview");

            migrationBuilder.DropColumn(
                name: "SourceGroupId",
                table: "PeerReview");

            migrationBuilder.RenameColumn(
                name: "TargetGroupId",
                table: "PeerReview",
                newName: "SubmissionId");

            migrationBuilder.RenameColumn(
                name: "TargetComponentId",
                table: "PeerReview",
                newName: "ComponentId");

            migrationBuilder.RenameIndex(
                name: "IX_PeerReview_TargetGroupId",
                table: "PeerReview",
                newName: "IX_PeerReview_SubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PeerReview_TargetComponentId",
                table: "PeerReview",
                newName: "IX_PeerReview_ComponentId");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseParticipantId",
                table: "PeerReview",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "PeerReview",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PeerReview_CourseParticipantId",
                table: "PeerReview",
                column: "CourseParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_PeerReview_GroupId",
                table: "PeerReview",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_PeerReview_AssignmentComponent_ComponentId",
                table: "PeerReview",
                column: "ComponentId",
                principalTable: "AssignmentComponent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PeerReview_CourseParticipant_CourseParticipantId",
                table: "PeerReview",
                column: "CourseParticipantId",
                principalTable: "CourseParticipant",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PeerReview_Group_GroupId",
                table: "PeerReview",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PeerReview_Submission_SubmissionId",
                table: "PeerReview",
                column: "SubmissionId",
                principalTable: "Submission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
