using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBSeniorLearnApp.UI.Data.Migrations.Service
{
    /// <inheritdoc />
    public partial class FixedLoginIssuesService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Firstname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaidStatus = table.Column<bool>(type: "bit", nullable: false),
                    MemberNumber = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    DateRegistered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdatedPaidStatus = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.UniqueConstraint("AK_Members_MemberNumber", x => x.MemberNumber);
                });

            migrationBuilder.CreateTable(
                name: "ProfessionalMembers",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalMembers", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_ProfessionalMembers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Prerequisites = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_ProfessionalMembers_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "ProfessionalMembers",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstructorId",
                table: "Courses",
                column: "InstructorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "ProfessionalMembers");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
