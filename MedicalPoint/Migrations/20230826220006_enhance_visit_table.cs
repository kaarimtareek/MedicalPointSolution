using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalPoint.Migrations
{
    /// <inheritdoc />
    public partial class enhance_visit_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowDate",
                table: "Visits");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Visits",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Visits");

            migrationBuilder.AddColumn<DateTime>(
                name: "FollowDate",
                table: "Visits",
                type: "datetime2",
                nullable: true);
        }
    }
}
