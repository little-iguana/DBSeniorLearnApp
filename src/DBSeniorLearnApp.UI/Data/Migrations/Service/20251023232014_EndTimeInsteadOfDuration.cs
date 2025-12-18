using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBSeniorLearnApp.UI.Data.Migrations.Service
{
    /// <inheritdoc />
    public partial class EndTimeInsteadOfDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Courses");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Courses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Courses");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
