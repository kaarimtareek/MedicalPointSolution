using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalPoint.Migrations
{
    /// <inheritdoc />
    public partial class craetepatients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Degree",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "RegisteredUserId",
                table: "Visits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "DegreeId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "Salt",
                table: "Users",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "BedNumber",
                table: "UnderObservationBeds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UnderObservationBeds",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EnterDate",
                table: "UnderObservationBedHistories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visits_RegisteredUserId",
                table: "Visits",
                column: "RegisteredUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DegreeId",
                table: "Users",
                column: "DegreeId");

            migrationBuilder.CreateIndex(
                name: "IX_UnderObservationBedHistories_BedId",
                table: "UnderObservationBedHistories",
                column: "BedId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnderObservationBedHistories_UnderObservationBeds_BedId",
                table: "UnderObservationBedHistories",
                column: "BedId",
                principalTable: "UnderObservationBeds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Degrees_DegreeId",
                table: "Users",
                column: "DegreeId",
                principalTable: "Degrees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Users_RegisteredUserId",
                table: "Visits",
                column: "RegisteredUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnderObservationBedHistories_UnderObservationBeds_BedId",
                table: "UnderObservationBedHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Degrees_DegreeId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Users_RegisteredUserId",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_RegisteredUserId",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Users_DegreeId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UnderObservationBedHistories_BedId",
                table: "UnderObservationBedHistories");

            migrationBuilder.DropColumn(
                name: "RegisteredUserId",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "DegreeId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Salt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BedNumber",
                table: "UnderObservationBeds");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UnderObservationBeds");

            migrationBuilder.DropColumn(
                name: "EnterDate",
                table: "UnderObservationBedHistories");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Degree",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
