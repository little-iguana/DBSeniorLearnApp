using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBSeniorLearnApp.UI.Data.Migrations.Service
{
    /// <inheritdoc />
    public partial class HopefullyFixedFKs_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalMembers_Members_MemberId",
                table: "ProfessionalMembers");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "ProfessionalMembers",
                newName: "StandardMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalMembers_Members_StandardMemberId",
                table: "ProfessionalMembers",
                column: "StandardMemberId",
                principalTable: "Members",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalMembers_Members_StandardMemberId",
                table: "ProfessionalMembers");

            migrationBuilder.RenameColumn(
                name: "StandardMemberId",
                table: "ProfessionalMembers",
                newName: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalMembers_Members_MemberId",
                table: "ProfessionalMembers",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");
        }
    }
}
