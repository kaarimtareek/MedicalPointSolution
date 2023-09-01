using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalPoint.Migrations
{
    /// <inheritdoc />
    public partial class add_isgiven_columns_in_visitmedicine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMedicinesGiven",
                table: "Visits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "MedicineGivenTime",
                table: "Visits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GivenTime",
                table: "VisitMedicines",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGiven",
                table: "VisitMedicines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMedicinesGiven",
                table: "VisitHistories",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MedicineGivenTime",
                table: "VisitHistories",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMedicinesGiven",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "MedicineGivenTime",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "GivenTime",
                table: "VisitMedicines");

            migrationBuilder.DropColumn(
                name: "IsGiven",
                table: "VisitMedicines");

            migrationBuilder.DropColumn(
                name: "IsMedicinesGiven",
                table: "VisitHistories");

            migrationBuilder.DropColumn(
                name: "MedicineGivenTime",
                table: "VisitHistories");
        }
    }
}
