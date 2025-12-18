using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBSeniorLearnApp.UI.Data.Migrations.Service
{
    /// <inheritdoc />
    public partial class RemovedInstructorMemberNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstructorMemberNumber",
                table: "Courses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstructorMemberNumber",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
