using System;
using System.Collections.Generic;
using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradingModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<LangStr>(type: "jsonb", nullable: false),
                    CourseNumber = table.Column<int>(type: "integer", nullable: false),
                    Semester = table.Column<int>(type: "integer", nullable: false),
                    EducationLevel = table.Column<string>(type: "text", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<LangStr>(type: "jsonb", nullable: false),
                    Description = table.Column<LangStr>(type: "jsonb", nullable: false),
                    FileIds = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsGroupAssignment = table.Column<bool>(type: "boolean", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignment_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<LangStr>(type: "jsonb", nullable: false),
                    Description = table.Column<LangStr>(type: "jsonb", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Group_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teacher",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teacher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teacher_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teacher_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentComponent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<LangStr>(type: "jsonb", nullable: false),
                    Description = table.Column<LangStr>(type: "jsonb", nullable: false),
                    FileIds = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    MaxPoints = table.Column<int>(type: "integer", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentComponent_Assignment_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseParticipant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseParticipant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseParticipant_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseParticipant_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseParticipant_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentGrade",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Grade = table.Column<int>(type: "integer", nullable: false),
                    Feedback = table.Column<LangStr>(type: "jsonb", nullable: true),
                    FileIds = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    GradedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GradedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseParticipantId = table.Column<Guid>(type: "uuid", nullable: true),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentGrade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentGrade_AssignmentComponent_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "AssignmentComponent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComponentGrade_CourseParticipant_CourseParticipantId",
                        column: x => x.CourseParticipantId,
                        principalTable: "CourseParticipant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ComponentGrade_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Text = table.Column<LangStr>(type: "jsonb", nullable: true),
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileIds = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    SubmittedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubmittedByGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    CourseParticipantId = table.Column<Guid>(type: "uuid", nullable: true),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submission_AssignmentComponent_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "AssignmentComponent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submission_CourseParticipant_CourseParticipantId",
                        column: x => x.CourseParticipantId,
                        principalTable: "CourseParticipant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Submission_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PeerReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedGrade = table.Column<int>(type: "integer", nullable: true),
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    CourseParticipantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeerReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeerReview_AssignmentComponent_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "AssignmentComponent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeerReview_CourseParticipant_CourseParticipantId",
                        column: x => x.CourseParticipantId,
                        principalTable: "CourseParticipant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PeerReview_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PeerReview_Submission_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_CourseId",
                table: "Assignment",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentComponent_AssignmentId",
                table: "AssignmentComponent",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentGrade_ComponentId",
                table: "ComponentGrade",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentGrade_CourseParticipantId",
                table: "ComponentGrade",
                column: "CourseParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentGrade_GroupId",
                table: "ComponentGrade",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseParticipant_CourseId",
                table: "CourseParticipant",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseParticipant_GroupId",
                table: "CourseParticipant",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseParticipant_UserId",
                table: "CourseParticipant",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Group_CourseId",
                table: "Group",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_PeerReview_ComponentId",
                table: "PeerReview",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_PeerReview_CourseParticipantId",
                table: "PeerReview",
                column: "CourseParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_PeerReview_GroupId",
                table: "PeerReview",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PeerReview_SubmissionId",
                table: "PeerReview",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_ComponentId",
                table: "Submission",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_CourseParticipantId",
                table: "Submission",
                column: "CourseParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_GroupId",
                table: "Submission",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_CourseId",
                table: "Teacher",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_UserId",
                table: "Teacher",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponentGrade");

            migrationBuilder.DropTable(
                name: "PeerReview");

            migrationBuilder.DropTable(
                name: "Teacher");

            migrationBuilder.DropTable(
                name: "Submission");

            migrationBuilder.DropTable(
                name: "AssignmentComponent");

            migrationBuilder.DropTable(
                name: "CourseParticipant");

            migrationBuilder.DropTable(
                name: "Assignment");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Course");
        }
    }
}
